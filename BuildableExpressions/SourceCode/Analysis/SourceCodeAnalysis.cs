namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Generics;
    using Operators;
    using ReadableExpressions;
    using static System.Linq.Expressions.ExpressionType;
    using static SourceCodeTranslationSettings;

    internal class SourceCodeAnalysis : ExpressionAnalysis
    {
        private readonly ReferenceAnalysis _referenceAnalysis;
        private readonly Stack<Expression> _expressions;
        private MethodScopeBase _currentMethodScope;

        private SourceCodeAnalysis()
            : base(Settings)
        {
            _referenceAnalysis = new ReferenceAnalysis();
            _expressions = new Stack<Expression>();
        }

        #region Factory Method

        public static SourceCodeAnalysis For(SourceCodeExpression expression)
        {
            var analysis = new SourceCodeAnalysis();
            analysis.Analyse(expression);

            return analysis;
        }

        #endregion

        public IList<Assembly> RequiredAssemblies
            => _referenceAnalysis.RequiredAssemblies;

        public IList<string> RequiredNamespaces
            => _referenceAnalysis.RequiredNamespaces;

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
                case (ExpressionType)SourceCodeExpressionType.GenericArgument:
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
                case Block when ExtractToMethod((BlockExpression)expression, out var extractedMethod):
                    return extractedMethod.CallExpression;

                case Default:
                    _referenceAnalysis.Visit((DefaultExpression)expression);
                    goto default;

                case (ExpressionType)SourceCodeExpressionType.SourceCode:
                    return VisitAndConvert((SourceCodeExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Type:
                    return VisitAndConvert((TypeExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Constructor:
                case (ExpressionType)SourceCodeExpressionType.Method:
                    return VisitAndConvert((MethodExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Field:
                    return VisitAndConvert((FieldExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Property:
                    return VisitAndConvert((PropertyExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.GenericArgument
                    when expression is GenericParameterExpression genericParameter:
                    {
                        return VisitAndConvert(genericParameter);
                    }

                case Extension when expression is NameOfOperatorExpression nameOf:
                    return nameOf.Update(VisitAndConvert(nameOf.Operand));

                default:
                    return base.VisitAndConvert(expression);
            }
        }

        private bool ExtractToMethod(BlockExpression block, out BlockMethodExpression extractedMethod)
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
                case (ExpressionType)SourceCodeExpressionType.Constructor:
                case Lambda:
                case Loop:
                case (ExpressionType)SourceCodeExpressionType.Method:
                case Quote:
                case Try:
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
            _referenceAnalysis.Visit(constant);
            return base.VisitAndConvert(constant);
        }

        private FieldExpression VisitAndConvert(FieldExpression field)
        {
            _referenceAnalysis.Visit(field);
            return field;
        }

        private TypeExpression VisitAndConvert(GenericParameterExpression parameter)
        {
            var method = _currentMethodScope.RootMethodExpression;

            if (method.GenericParametersAccessor?.Contains(parameter) == true)
            {
                return parameter;
            }

            var closedType = method
                .DeclaringTypeExpression
                .TryGetTypeExpressionFor(parameter, out var typeExpression);

            return closedType ? typeExpression : parameter;
        }

        private SourceCodeExpression VisitAndConvert(SourceCodeExpression sourceCode)
        {
            foreach (Expression typeExpression in sourceCode.TypeExpressions)
            {
                VisitAndConvert(typeExpression);
            }

            sourceCode.Finalise();
            return sourceCode;
        }

        private TypeExpression VisitAndConvert(TypeExpression type)
        {
            _referenceAnalysis.Visit(type);

            foreach (var memberExpression in type.MemberExpressionsAccessor)
            {
                VisitAndConvert(memberExpression);
            }

            return type;
        }

        protected override Expression VisitAndConvert(MemberExpression memberAccess)
        {
            _referenceAnalysis.Visit(memberAccess);
            return base.VisitAndConvert(memberAccess);
        }

        protected override Expression VisitAndConvert(MethodCallExpression methodCall)
        {
            _referenceAnalysis.Visit(methodCall);
            return base.VisitAndConvert(methodCall);
        }

        private MethodExpression VisitAndConvert(MethodExpression method)
        {
            EnterMethodScope(method);

            VisitAndConvert(method.Parameters);
            var updatedBody = VisitAndConvert(method.Body);

            _currentMethodScope.Finalise(updatedBody);
            _referenceAnalysis.Visit(method);

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
            _referenceAnalysis.Visit(newArray);
            return base.VisitAndConvert(newArray);
        }

        protected override Expression VisitAndConvert(NewExpression newing)
        {
            _referenceAnalysis.Visit(newing);
            return base.VisitAndConvert(newing);
        }

        protected override Expression VisitAndConvert(ParameterExpression variable)
        {
            _currentMethodScope.VariableAccessed(variable);
            return base.VisitAndConvert(variable);
        }

        private PropertyExpression VisitAndConvert(PropertyExpression property)
        {
            _referenceAnalysis.Visit(property);
            return property;
        }

        protected override CatchBlock VisitAndConvert(CatchBlock @catch)
        {
            _referenceAnalysis.Visit(@catch);

            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                _currentMethodScope.Add(catchVariable);
            }

            return base.VisitAndConvert(@catch);
        }

        protected override ExpressionAnalysis Finalise()
        {
            _referenceAnalysis.Finalise();
            return base.Finalise();
        }
    }
}