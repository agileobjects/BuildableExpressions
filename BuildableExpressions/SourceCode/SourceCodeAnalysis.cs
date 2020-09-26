namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;
    using static System.Linq.Expressions.ExpressionType;

    internal class SourceCodeAnalysis : ExpressionAnalysis
    {
        private readonly SourceCodeTranslationSettings _settings;
        private readonly Stack<Expression> _expressions;
        private List<string> _requiredNamespaces;
        private MethodScope _currentMethodScope;
        private Dictionary<Expression, MethodExpression> _methodsByConvertedBlock;

        private SourceCodeAnalysis(SourceCodeTranslationSettings settings)
            : base(settings)
        {
            _settings = settings;
            _expressions = new Stack<Expression>();
        }

        #region Factory Method

        public static SourceCodeAnalysis For(
            SourceCodeExpression expression,
            SourceCodeTranslationSettings settings)
        {
            var analysis = new SourceCodeAnalysis(settings);

            analysis.ResultExpression = analysis.VisitAndConvert((Expression)expression);
            analysis.Finalise();

            return analysis;
        }

        protected override ExpressionAnalysis Finalise()
        {
            if (_requiredNamespaces != null)
            {
                _requiredNamespaces.Sort(UsingsComparer.Instance);
            }
            else if (_settings.CollectRequiredNamespaces)
            {
                _requiredNamespaces = Enumerable<string>.EmptyList;
            }

            return base.Finalise();
        }

        #endregion

        public IList<string> RequiredNamespaces => _requiredNamespaces;

        public bool IsMethodBlock(BlockExpression block, out MethodExpression blockMethod)
        {
            if (_methodsByConvertedBlock == null)
            {
                blockMethod = null;
                return false;
            }

            return _methodsByConvertedBlock.TryGetValue(block, out blockMethod);
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

            switch (expression.NodeType)
            {
                case Block when ExtractToMethod((BlockExpression)expression):
                    goto SkipBaseVisit;

                case Default:
                    AddNamespaceIfRequired(expression);
                    break;

                case (ExpressionType)SourceCodeExpressionType.SourceCode:
                    expression = VisitAndConvert((SourceCodeExpression)expression);
                    goto SkipBaseVisit;

                case (ExpressionType)SourceCodeExpressionType.Class:
                    expression = VisitAndConvert((ClassExpression)expression);
                    goto SkipBaseVisit;

                case (ExpressionType)SourceCodeExpressionType.Method:
                    expression = VisitAndConvert((MethodExpression)expression);
                    goto SkipBaseVisit;

                case (ExpressionType)SourceCodeExpressionType.MethodParameter:
                    Visit((MethodParameterExpression)expression);
                    goto SkipBaseVisit;
            }

            expression = base.VisitAndConvert(expression);

        SkipBaseVisit:

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

        private bool ExtractToMethod(Expression block)
        {
            if (!Extract(block))
            {
                return false;
            }

            var blockMethod = _currentMethodScope.CreateMethodFor(block);
            var updatedMethod = VisitAndConvert(blockMethod);

            (_methodsByConvertedBlock ??= new Dictionary<Expression, MethodExpression>())
                .Add(block, updatedMethod);

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

        private SourceCodeExpression VisitAndConvert(SourceCodeExpression sourceCode)
        {
            sourceCode.Finalise(VisitAndConvert(
                sourceCode.Classes,
                c => (ClassExpression)VisitAndConvert((Expression)c)));

            return sourceCode;
        }

        private ClassExpression VisitAndConvert(ClassExpression @class)
        {
            AddNamespacesIfRequired(@class.Interfaces);

            VisitAndConvert(
                @class.Methods,
                m => (MethodExpression)VisitAndConvert((Expression)m));

            return @class;
        }

        protected override Expression VisitAndConvert(ConstantExpression constant)
        {
            if (constant.Type.IsEnum())
            {
                AddNamespaceIfRequired(constant);
            }
            else if (constant.Type.IsAssignableTo(typeof(Type)))
            {
                AddNamespaceIfRequired((Type)constant.Value);
            }

            return constant;
        }

        protected override Expression VisitAndConvert(MemberExpression memberAccess)
        {
            if (memberAccess.Expression == null)
            {
                // Static member access
                AddNamespaceIfRequired(memberAccess.Member.DeclaringType);
            }

            return base.VisitAndConvert(memberAccess);
        }

        protected override Expression VisitAndConvert(MethodCallExpression methodCall)
        {
            if (methodCall.Method.IsGenericMethod)
            {
                AddNamespacesIfRequired(new BclMethodWrapper(methodCall.Method)
                    .GetRequiredExplicitGenericArguments(_settings));
            }

            if (methodCall.Method.IsStatic)
            {
                AddNamespaceIfRequired(methodCall.Method.DeclaringType);
            }

            return base.VisitAndConvert(methodCall);
        }

        private MethodExpression VisitAndConvert(MethodExpression method)
        {
            EnterMethodScope(method);

            AddNamespaceIfRequired(method);

            var updatedParameters = VisitAndConvert(method.Parameters, Visit);
            var updatedBody = VisitAndConvert(method.Body);

            _currentMethodScope.Finalise(updatedBody, updatedParameters);

            ExitMethodScope();
            return method;
        }

        private void EnterMethodScope(MethodExpression method)
            => _currentMethodScope = new MethodScope(method, _currentMethodScope);

        private void ExitMethodScope()
            => _currentMethodScope = _currentMethodScope.Parent;

        private MethodParameterExpression Visit(MethodParameterExpression methodParameter)
        {
            AddNamespaceIfRequired(methodParameter);
            return methodParameter;
        }

        protected override Expression VisitAndConvert(NewArrayExpression newArray)
        {
            AddNamespaceIfRequired(newArray.Type.GetElementType());
            return base.VisitAndConvert(newArray);
        }

        protected override Expression VisitAndConvert(NewExpression newing)
        {
            AddNamespaceIfRequired(newing.Type);
            return base.VisitAndConvert(newing);
        }

        protected override Expression VisitAndConvert(ParameterExpression variable)
        {
            _currentMethodScope.VariableAccessed(variable);
            return base.VisitAndConvert(variable);
        }

        protected override CatchBlock VisitAndConvert(CatchBlock @catch)
        {
            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                AddNamespaceIfRequired(catchVariable);
                _currentMethodScope.Add(catchVariable);
            }

            return base.VisitAndConvert(@catch);
        }

        private void AddNamespacesIfRequired(IEnumerable<Type> accessedTypes)
        {
            foreach (var type in accessedTypes)
            {
                AddNamespaceIfRequired(type);
            }
        }

        private void AddNamespaceIfRequired(Expression expression)
            => AddNamespaceIfRequired(expression.Type);

        private void AddNamespaceIfRequired(Type accessedType)
        {
            if (!_settings.CollectRequiredNamespaces ||
                (accessedType == typeof(void)) ||
                (accessedType == typeof(string)) ||
                (accessedType == typeof(object)) ||
                accessedType.IsPrimitive())
            {
                return;
            }

            if (accessedType.IsGenericType())
            {
                AddNamespacesIfRequired(accessedType.GetGenericTypeArguments());
            }

            var @namespace = accessedType.Namespace;

            if (@namespace == null)
            {
                return;
            }

            _requiredNamespaces ??= new List<string>();

            if (!_requiredNamespaces.Contains(@namespace))
            {
                _requiredNamespaces.Add(@namespace);
            }
        }

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
                => _method.Parent.AddMethod(block, MemberVisibility.Private);

            public void Finalise(
                Expression updatedBody,
                IList<MethodParameterExpression> updatedParameters)
            {
                if (_unscopedVariables.Any())
                {
                    if (updatedParameters.IsReadOnly)
                    {
                        updatedParameters = new List<MethodParameterExpression>(
                            updatedParameters.Count + _unscopedVariables.Count);
                    }

                    foreach (var variable in _unscopedVariables)
                    {
                        updatedParameters.Add(new MethodParameterExpression(variable));
                    }
                }

                _method.Finalise(updatedBody, updatedParameters);
            }
        }
    }
}