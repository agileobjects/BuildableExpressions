namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Generics;
    using NetStandardPolyfills;
    using Operators;
    using ReadableExpressions;
    using static System.Linq.Expressions.ExpressionType;
    using static SourceCodeTranslationSettings;

    internal class SourceCodeAnalysis : ExpressionAnalysis
    {
        private readonly bool _sourceCodeComplete;
        private readonly ReferenceAnalysis _referenceAnalysis;
        private readonly Stack<Expression> _expressions;
        private MethodScopeBase _currentMethodScope;
        private Dictionary<Expression, IList<ParameterExpression>> _methodParametersByBlock;

        private SourceCodeAnalysis(SourceCodeExpression expression)
            : base(Settings)
        {
            _sourceCodeComplete = expression.IsComplete;
            _referenceAnalysis = new ReferenceAnalysis();
            _expressions = new Stack<Expression>();
        }

        #region Factory Method

        public static SourceCodeAnalysis For(SourceCodeExpression expression)
        {
            var analysis = new SourceCodeAnalysis(expression);
            analysis.Analyse(expression);

            return analysis;
        }

        #endregion

        public IList<Assembly> RequiredAssemblies
            => _referenceAnalysis.RequiredAssemblies;

        public IList<string> RequiredNamespaces
            => _referenceAnalysis.RequiredNamespaces;

        protected override void Analyse(Expression expression)
        {
            base.Analyse(expression);
            _referenceAnalysis.Finalise();
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

                case ExpressionType.Convert when ((UnaryExpression)expression).Method?.IsImplicitOperator() != true:
                    _referenceAnalysis.Visit(expression);
                    goto default;

                case Default:
                    _referenceAnalysis.Visit((DefaultExpression)expression);
                    goto default;

                case Try when ExtractToMethod((TryExpression)expression, out var extractedMethod):
                    return extractedMethod.CallExpression;

                case (ExpressionType)SourceCodeExpressionType.SourceCode:
                    return VisitAndConvert((SourceCodeExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Type:
                    return VisitAndConvert((TypeExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Constructor:
                case (ExpressionType)SourceCodeExpressionType.Method:
                    return VisitAndConvert((MethodExpressionBase)expression);

                case (ExpressionType)SourceCodeExpressionType.Field:
                    return _referenceAnalysis.Visit((FieldExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Property:
                    return _referenceAnalysis.Visit((PropertyExpression)expression);

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

        #region Method Extraction

        private bool ExtractToMethod(BlockExpression block, out BlockMethodExpression extractedMethod)
        {
            if (Extract(block, b => b.Expressions.Any() || b.Variables.Any()))
            {
                ExtractToMethod(block, out extractedMethod, VisitAndConvert);
                return true;
            }

            extractedMethod = null;
            return false;
        }

        private bool ExtractToMethod(TryExpression @try, out BlockMethodExpression extractedMethod)
        {
            if (Extract(@try, _ => true))
            {
                ExtractToMethod(@try, out extractedMethod, VisitAndConvert);
                return true;
            }

            extractedMethod = null;
            return false;
        }

        private void ExtractToMethod<TExpression>(
            TExpression expression,
            out BlockMethodExpression extractedMethod,
            Func<TExpression, Expression> visitAndConvert)
            where TExpression : Expression
        {
            var blockMethodScope = new BlockMethodScope(_currentMethodScope);

            EnterMethodScope(blockMethodScope);

            var updatedExpression = visitAndConvert.Invoke(expression);

            blockMethodScope.Finalise(updatedExpression);

            extractedMethod = blockMethodScope.BlockMethod;

            if (extractedMethod.ParametersAccessor != null && expression.NodeType == Block)
            {
                (_methodParametersByBlock ??= new Dictionary<Expression, IList<ParameterExpression>>())
                    .Add(expression, extractedMethod.ParametersAccessor);
            }

            ExitMethodScope();
        }

        private bool Extract<TExpression>(
            TExpression expression,
            Func<TExpression, bool> extractPredicate)
            where TExpression : Expression
        {
            if (!_sourceCodeComplete)
            {
                return false;
            }

            var parentExpression = _expressions.ElementAt(1);

            switch (parentExpression.NodeType)
            {
                case Block:
                case (ExpressionType)SourceCodeExpressionType.Constructor:
                case Loop:
                case (ExpressionType)SourceCodeExpressionType.Method:
                case Try:
                    return false;

                case Conditional:
                    var conditional = (ConditionalExpression)parentExpression;

                    if (conditional.Type != typeof(void))
                    {
                        // Ternary
                        goto default;
                    }

                    if (expression == conditional.IfTrue || expression == conditional.IfFalse)
                    {
                        return false;
                    }

                    goto default;

                case Lambda:
                    return _expressions.ElementAtOrDefault(2)?.NodeType == Call;

                case Switch:
                    var @switch = (SwitchExpression)parentExpression;

                    if (expression == @switch.DefaultBody ||
                        @switch.Cases.Any(@case => expression == @case.Body))
                    {
                        return false;
                    }

                    goto default;

                default:
                    return extractPredicate.Invoke(expression);
            }
        }

        #endregion

        protected override Expression VisitAndConvert(BlockExpression block)
        {
            _currentMethodScope.Add(block.Variables);
            return base.VisitAndConvert(block);
        }

        protected override Expression VisitAndConvert(ConstantExpression constant)
        {
            _referenceAnalysis.Visit(constant);
            return base.VisitAndConvert(constant);
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

        protected override LambdaExpression VisitAndConvert(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                return null;
            }

            _currentMethodScope.Add(lambda.Parameters);
            return base.VisitAndConvert(lambda);
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

        private MethodExpressionBase VisitAndConvert(MethodExpressionBase method)
        {
            EnterMethodScope(method);

            VisitAndConvert(method.Parameters);
            var updatedBody = VisitAndConvert(method.Body);

            _currentMethodScope.Finalise(updatedBody);
            _referenceAnalysis.Visit(method);

            ExitMethodScope();
            return method;
        }

        #region Method Scoping

        private void EnterMethodScope(MethodExpressionBase method)
            => EnterMethodScope(new MethodExpressionScope(method, _currentMethodScope));

        private void EnterMethodScope(MethodScopeBase methodScope)
            => _currentMethodScope = methodScope;

        private void ExitMethodScope()
            => _currentMethodScope = _currentMethodScope.Parent;

        #endregion

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

        private SourceCodeExpression VisitAndConvert(SourceCodeExpression sourceCode)
        {
            foreach (var typeExpression in sourceCode.TypeExpressions)
            {
                VisitAndConvert((Expression)typeExpression);
            }

            return sourceCode;
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

        private TypeExpression VisitAndConvert(TypeExpression type)
        {
            _referenceAnalysis.Visit(type);

            foreach (var memberExpression in type.MemberExpressionsAccessor)
            {
                VisitAndConvert(memberExpression);
            }

            type.Finalise();
            return type;
        }

        public override bool ShouldBeDeclaredInVariableList(
            ParameterExpression variable,
            BlockExpression block)
        {
            return base.ShouldBeDeclaredInVariableList(variable, block) &&
                   _methodParametersByBlock?[block].Contains(variable) != true;
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
    }
}