namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using static System.Linq.Expressions.ExpressionType;
    using static MemberVisibility;

    internal class SourceCodeAnalysis : ExpressionAnalysis
    {
        private readonly NamespaceAnalysis _namespaceAnalysis;
        private readonly Stack<Expression> _expressions;
        private MethodScope _currentMethodScope;

        private SourceCodeAnalysis(SourceCodeTranslationSettings settings)
            : base(settings)
        {
            _namespaceAnalysis = new NamespaceAnalysis(settings);
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
            _namespaceAnalysis.Finalise();
            return base.Finalise();
        }

        #endregion

        public IList<string> RequiredNamespaces
            => _namespaceAnalysis.RequiredNamespaces;

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
                case Block when ExtractToMethod((BlockExpression)expression, out var extractedMethod):
                    expression = BuildableExpression.Call(extractedMethod, extractedMethod.Parameters);
                    goto SkipBaseVisit;

                case Default:
                    _namespaceAnalysis.Visit((DefaultExpression)expression);
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

        private SourceCodeExpression VisitAndConvert(SourceCodeExpression sourceCode)
        {
            sourceCode.Finalise(VisitAndConvert(
                sourceCode.Classes,
                c => (ClassExpression)VisitAndConvert((Expression)c)));

            return sourceCode;
        }

        private ClassExpression VisitAndConvert(ClassExpression @class)
        {
            _namespaceAnalysis.Visit(@class);

            VisitAndConvert(
                @class.Methods,
                m => (MethodExpression)VisitAndConvert((Expression)m));

            return @class;
        }

        protected override Expression VisitAndConvert(ConstantExpression constant)
        {
            _namespaceAnalysis.Visit(constant);
            return base.VisitAndConvert(constant);
        }

        protected override Expression VisitAndConvert(MemberExpression memberAccess)
        {
            _namespaceAnalysis.Visit(memberAccess);
            return base.VisitAndConvert(memberAccess);
        }

        protected override Expression VisitAndConvert(MethodCallExpression methodCall)
        {
            _namespaceAnalysis.Visit(methodCall);
            return base.VisitAndConvert(methodCall);
        }

        private MethodExpression VisitAndConvert(MethodExpression method)
        {
            EnterMethodScope(method);

            var updatedParameters = VisitAndConvert(method.Parameters);
            var updatedBody = VisitAndConvert(method.Body);

            _currentMethodScope.Finalise(updatedBody, updatedParameters);
            _namespaceAnalysis.Visit(method);

            ExitMethodScope();
            return method;
        }

        private void EnterMethodScope(MethodExpression method)
            => _currentMethodScope = new MethodScope(method, _currentMethodScope);

        private void ExitMethodScope()
            => _currentMethodScope = _currentMethodScope.Parent;

        protected override Expression VisitAndConvert(NewArrayExpression newArray)
        {
            _namespaceAnalysis.Visit(newArray);
            return base.VisitAndConvert(newArray);
        }

        protected override Expression VisitAndConvert(NewExpression newing)
        {
            _namespaceAnalysis.Visit(newing);
            return base.VisitAndConvert(newing);
        }

        protected override Expression VisitAndConvert(ParameterExpression variable)
        {
            _currentMethodScope.VariableAccessed(variable);
            return base.VisitAndConvert(variable);
        }

        protected override CatchBlock VisitAndConvert(CatchBlock @catch)
        {
            _namespaceAnalysis.Visit(@catch);

            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                _currentMethodScope.Add(catchVariable);
            }

            return base.VisitAndConvert(@catch);
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
                => _method.Class.AddMethod(block, m => m.WithVisibility(Private));

            public void Finalise(
                Expression updatedBody,
                IList<ParameterExpression> updatedParameters)
            {
                if (_unscopedVariables.Any())
                {
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
    }
}