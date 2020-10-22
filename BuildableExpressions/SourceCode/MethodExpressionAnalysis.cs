namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using static System.Linq.Expressions.ExpressionType;
    using static System.StringComparison;
    using static SourceCodeTranslationSettings;

    internal class MethodExpressionAnalysis : ExpressionAnalysis
    {
        private readonly Stack<Expression> _expressions;
        private MethodScopeBase _currentMethodScope;

        private MethodExpressionAnalysis(NamespaceAnalysis namespaceAnalysis)
            : base(Settings)
        {
            NamespaceAnalysis = namespaceAnalysis;
            _expressions = new Stack<Expression>();
        }

        #region Factory Methods

        public static MethodExpressionAnalysis For(MethodExpression method)
            => For(method, new NamespaceAnalysis());

        public static MethodExpressionAnalysis For(
            MethodExpression method,
            NamespaceAnalysis namespaceAnalysis)
        {
            if (method.Analysis != null)
            {
                return method.Analysis;
            }

            var analysis = new MethodExpressionAnalysis(namespaceAnalysis);
            analysis.Analyse(method);

            return analysis;
        }

        #endregion

        public NamespaceAnalysis NamespaceAnalysis { get; }

        protected override Expression VisitAndConvert(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            var isParentNode = HasChildNodes(expression);

            if (isParentNode)
            {
                _expressions.Push(expression);
            }

            expression = VisitAndConvertCore(expression);

            if (isParentNode)
            {
                _expressions.Pop();
            }

            return expression;
        }

        private static bool HasChildNodes(Expression expression)
        {
            switch (expression.NodeType)
            {
                case Constant:
                case Default:
                case DebugInfo:
                case Parameter:
                    return false;

                default:
                    return true;
            }
        }

        private Expression VisitAndConvertCore(Expression expression)
        {
            switch (expression.NodeType)
            {
                case Call when expression is BuildableMethodCallExpression methodCall:
                    VisitAndConvert((ICustomAnalysableExpression)methodCall);
                    return methodCall;

                case Block when ExtractToMethod((BlockExpression)expression, out var extractedMethod):
                    return BuildableExpression.Call(extractedMethod, extractedMethod.Parameters);

                case Default when expression is DefaultExpression @default:
                    NamespaceAnalysis?.Visit(@default);
                    goto default;

                case (ExpressionType)SourceCodeExpressionType.Method:
                    return VisitAndConvert((MethodExpression)expression);

                default:
                    return base.VisitAndConvert(expression);
            }
        }

        private bool ExtractToMethod(BlockExpression block, out MethodExpression extractedMethod)
        {
            if (!Extract(block))
            {
                extractedMethod = null;
                return false;
            }

            var blockMethodScope = new BlockMethodScope(_currentMethodScope);

            EnterMethodScope(blockMethodScope);

            var updatedBlock = VisitAndConvert(block);

            blockMethodScope.Finalise(updatedBlock);

            extractedMethod = blockMethodScope.BlockMethod;

            ExitMethodScope();
            return true;
        }

        private bool Extract(Expression blockExpression)
        {
            var parentExpression = _expressions.ElementAt(1);

            switch (parentExpression.NodeType)
            {
                case Block:
                case Lambda:
                case Loop:
                case Quote:
                case Try:
                case (ExpressionType)SourceCodeExpressionType.Method:
                    return false;

                case Switch:
                    var @switch = (SwitchExpression)parentExpression;

                    if (blockExpression == @switch.DefaultBody ||
                        @switch.Cases.Any(@case => blockExpression == @case.Body))
                    {
                        return false;
                    }

                    goto default;

                default:
                    var block = (BlockExpression)blockExpression;
                    return block.Expressions.Any() || block.Variables.Any();
            }
        }

        protected override Expression VisitAndConvert(BlockExpression block)
        {
            _currentMethodScope.Add(block.Variables);
            return base.VisitAndConvert(block);
        }

        protected override bool IsAssignmentJoinable(ParameterExpression variable)
        {
            _currentMethodScope.VariableAccessed(variable);

            if (_currentMethodScope.IsMethodParameter(variable))
            {
                return false;
            }

            return base.IsAssignmentJoinable(variable);
        }

        protected override Expression VisitAndConvert(ConstantExpression constant)
        {
            NamespaceAnalysis?.Visit(constant);
            return base.VisitAndConvert(constant);
        }

        protected override Expression VisitAndConvert(MemberExpression memberAccess)
        {
            NamespaceAnalysis?.Visit(memberAccess);
            return base.VisitAndConvert(memberAccess);
        }

        protected override Expression VisitAndConvert(MethodCallExpression methodCall)
        {
            NamespaceAnalysis?.Visit(methodCall);
            return base.VisitAndConvert(methodCall);
        }

        private MethodExpression VisitAndConvert(MethodExpression method)
        {
            EnterMethodScope(method);

            VisitAndConvert(method.Parameters);
            var updatedBody = VisitAndConvert(method.Body);

            _currentMethodScope.Finalise(updatedBody);
            NamespaceAnalysis?.Visit(method);

            ExitMethodScope();
            return method;
        }

        private void EnterMethodScope(MethodExpression method)
            => EnterMethodScope(new MethodExpressionScope(method, _currentMethodScope));

        private void EnterMethodScope(MethodScopeBase methodScope)
            => _currentMethodScope = methodScope;

        private void ExitMethodScope()
            => _currentMethodScope = _currentMethodScope.Parent;

        protected override Expression VisitAndConvert(NewArrayExpression newArray)
        {
            NamespaceAnalysis?.Visit(newArray);
            return base.VisitAndConvert(newArray);
        }

        protected override Expression VisitAndConvert(NewExpression newing)
        {
            NamespaceAnalysis?.Visit(newing);
            return base.VisitAndConvert(newing);
        }

        protected override Expression VisitAndConvert(ParameterExpression variable)
        {
            _currentMethodScope.VariableAccessed(variable);
            return base.VisitAndConvert(variable);
        }

        protected override CatchBlock VisitAndConvert(CatchBlock @catch)
        {
            NamespaceAnalysis?.Visit(@catch);

            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                _currentMethodScope.Add(catchVariable);
            }

            return base.VisitAndConvert(@catch);
        }

        #region Helper Class

        private abstract class MethodScopeBase
        {
            private readonly List<ParameterExpression> _inScopeVariables;
            private readonly List<ParameterExpression> _unscopedVariables;

            protected MethodScopeBase(MethodScopeBase parent)
            {
                Parent = parent;
                _inScopeVariables = new List<ParameterExpression>();
                _unscopedVariables = new List<ParameterExpression>();
            }

            public MethodScopeBase Parent { get; }

            public abstract TypeExpression GetDeclaringType();

            public void Add(ParameterExpression inScopeVariable)
                => _inScopeVariables.Add(inScopeVariable);

            public void Add(IEnumerable<ParameterExpression> inScopeVariables)
                => _inScopeVariables.AddRange(inScopeVariables);

            public void VariableAccessed(ParameterExpression variable)
            {
                if (_inScopeVariables.Contains(variable) ||
                    _unscopedVariables.Contains(variable))
                {
                    return;
                }

                UnscopedVariableAccessed(variable);
            }

            protected virtual void UnscopedVariableAccessed(ParameterExpression variable)
                => _unscopedVariables.Add(variable);

            public bool IsMethodParameter(ParameterExpression parameter)
                => _unscopedVariables.Contains(parameter);

            public virtual void Finalise(Expression finalisedBody)
            {
                if (_unscopedVariables.Any())
                {
                    UnscopedVariablesAccessed(_unscopedVariables);
                }
            }

            protected abstract void UnscopedVariablesAccessed(
                IEnumerable<ParameterExpression> unscopedVariables);
        }

        private class MethodExpressionScope : MethodScopeBase
        {
            private readonly MethodExpression _methodExpression;

            public MethodExpressionScope(MethodExpression methodExpression, MethodScopeBase parent)
                : base(parent)
            {
                _methodExpression = methodExpression;
                Add(methodExpression.Parameters);
            }

            public override TypeExpression GetDeclaringType()
                => _methodExpression.DeclaringTypeExpression;

            public override void Finalise(Expression finalisedBody)
            {
                base.Finalise(finalisedBody);
                _methodExpression.Update(finalisedBody);
            }

            protected override void UnscopedVariablesAccessed(
                IEnumerable<ParameterExpression> unscopedVariables)
            {
                var variables = string.Join(", ", unscopedVariables
                    .Select(v => $"'{v.Type.GetFriendlyName()} {v.Name}'"));

                throw new NotSupportedException(
                    $"Method accesses undefined variable(s) {variables}");
            }
        }

        private class BlockMethodScope : MethodScopeBase
        {
            private readonly bool _isNestedBlock;
            private List<ParameterExpression> _parameters;
            private List<BlockMethodScope> _childBlockScopes;

            public BlockMethodScope(MethodScopeBase parent)
                : base(parent)
            {
                if (parent is BlockMethodScope parentBlockScope)
                {
                    _isNestedBlock = true;
                    parentBlockScope.AddChildBlockScope(this);
                }
            }

            public MethodExpression BlockMethod { get; private set; }

            public override TypeExpression GetDeclaringType() => Parent.GetDeclaringType();

            private void AddChildBlockScope(BlockMethodScope childBlockScope)
            {
                _childBlockScopes ??= new List<BlockMethodScope>();
                _childBlockScopes.Add(childBlockScope);
            }

            protected override void UnscopedVariableAccessed(ParameterExpression variable)
            {
                base.UnscopedVariableAccessed(variable);
                Parent.VariableAccessed(variable);
            }

            public override void Finalise(Expression finalisedBody)
            {
                base.Finalise(finalisedBody);

                var declaringType = GetDeclaringType();

                BlockMethod = new MethodExpression(declaringType, m =>
                {
                    m.SetVisibility(MemberVisibility.Private);

                    if (_parameters != null)
                    {
                        m.AddParameters(_parameters);
                    }

                    m.SetBody(finalisedBody);
                });

                if (_isNestedBlock)
                {
                    return;
                }

                Finalise(declaringType);

                if (_childBlockScopes == null)
                {
                    return;
                }

                foreach (var childBlockScope in _childBlockScopes)
                {
                    childBlockScope.Finalise(declaringType);
                }
            }

            private void Finalise(TypeExpression declaringType)
            {
                BlockMethod.Name = GetName();
                declaringType.Register(BlockMethod);
            }

            private string GetName()
            {
                var baseName = GetBaseName();

                var latestMatchingMethodSuffix =
                    GetLatestMatchingMethodSuffix(baseName);

                if (latestMatchingMethodSuffix == 0)
                {
                    return baseName;
                }

                return baseName + (latestMatchingMethodSuffix + 1);
            }

            private int GetLatestMatchingMethodSuffix(string baseName)
            {
                var parameterTypes =
                    _parameters?.Project(p => p.Type).ToList() ??
                     Enumerable<Type>.EmptyList;

                return BlockMethod
                    .DeclaringTypeExpression
                    .MethodExpressions
                    .Filter(m => m.Name != null)
                    .Select(m =>
                    {
                        if (m.Name == baseName)
                        {
                            if (m.IsBlockMethod)
                            {
                                m.Name += "1";
                            }

                            return new { Suffix = 1 };
                        }

                        if (!m.Name.StartsWith(baseName, Ordinal))
                        {
                            return null;
                        }

                        var methodNameSuffix = m.Name.Substring(baseName.Length);

                        if (!int.TryParse(methodNameSuffix, out var suffix))
                        {
                            return null;
                        }

                        if (!m.Parameters.Project(p => p.Type).SequenceEqual(parameterTypes))
                        {
                            return null;
                        }

                        return new { Suffix = suffix };
                    })
                    .Filter(_ => _ != null)
                    .Select(_ => _.Suffix)
                    .OrderByDescending(suffix => suffix)
                    .FirstOrDefault();
            }

            private string GetBaseName()
            {
                var body = BlockMethod.Definition.Body;

                return body.HasReturnType()
                    ? "Get" + body.Type.GetVariableNameInPascalCase()
                    : "DoAction";
            }

            protected override void UnscopedVariablesAccessed(
                IEnumerable<ParameterExpression> unscopedVariables)
            {
                _parameters = new List<ParameterExpression>(unscopedVariables);
            }
        }

        #endregion
    }
}