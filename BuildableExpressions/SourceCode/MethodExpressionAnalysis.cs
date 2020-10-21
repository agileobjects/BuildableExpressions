namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using static System.Linq.Expressions.ExpressionType;
    using static MemberVisibility;
    using static SourceCodeTranslationSettings;

    internal class MethodExpressionAnalysis : ExpressionAnalysis
    {
        private readonly Stack<Expression> _expressions;
        private MethodScope _currentMethodScope;

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

        public void Analyse(MethodExpression method)
        {
            method.Analysis = this;
            base.Analyse(method);
        }

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

        private bool ExtractToMethod(Expression block, out MethodExpression extractedMethod)
        {
            if (!Extract(block))
            {
                extractedMethod = null;
                return false;
            }

            extractedMethod = _currentMethodScope.CreateMethodFor(block);
            extractedMethod = VisitAndConvert(extractedMethod);
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

            var updatedParameters = VisitAndConvert(method.Parameters);
            var updatedBody = VisitAndConvert(method.Body);

            _currentMethodScope.Finalise(updatedBody, updatedParameters);
            NamespaceAnalysis?.Visit(method);

            ExitMethodScope();
            return method;
        }

        private void EnterMethodScope(MethodExpression method)
            => _currentMethodScope = new MethodScope(method, _currentMethodScope);

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

        private class MethodScope
        {
            private readonly MethodExpression _method;
            private readonly List<ParameterExpression> _inScopeVariables;
            private readonly IList<ParameterExpression> _unscopedVariables;

            public MethodScope(MethodExpression method, MethodScope parent)
            {
                _method = method;
                Parent = parent;
                _inScopeVariables = new List<ParameterExpression>(method.Definition.Parameters);
                _unscopedVariables = new List<ParameterExpression>();
            }

            public MethodScope Parent { get; }

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

                _unscopedVariables.Add(variable);
                Parent?.VariableAccessed(variable);
            }

            public bool IsMethodParameter(ParameterExpression parameter)
            {
                VariableAccessed(parameter);

                return _unscopedVariables.Contains(parameter);
            }

            public MethodExpression CreateMethodFor(Expression block)
            {
                return _method.DeclaringTypeExpression
                    .AddMethod(block, m => m.SetVisibility(Private));
            }

            public void Finalise(
                Expression updatedBody,
                IList<ParameterExpression> updatedParameters)
            {
                if (_unscopedVariables.Any())
                {
                    if (Parent == null)
                    {
                        var variables = string.Join(", ", _unscopedVariables
                            .Select(v => $"'{v.Type.GetFriendlyName()} {v.Name}'"));

                        throw new NotSupportedException(
                            $"Method accesses undefined variable(s) {variables}");
                    }

                    if (updatedParameters.IsReadOnly)
                    {
                        updatedParameters = new List<ParameterExpression>(
                            updatedParameters.Count + _unscopedVariables.Count);
                    }

                    foreach (var variable in _unscopedVariables)
                    {
                        updatedParameters.Add(variable);
                    }
                }

                _method.Finalise(updatedBody, updatedParameters);
            }
        }

        #endregion
    }
}