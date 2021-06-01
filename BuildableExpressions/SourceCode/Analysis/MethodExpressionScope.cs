namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;

    internal class MethodExpressionScope : MethodScopeBase
    {
        private readonly MethodExpressionBase _methodExpression;

        public MethodExpressionScope(MethodExpressionBase methodExpression, MethodScopeBase parent)
            : base(parent)
        {
            _methodExpression = methodExpression;
            Add(methodExpression.Parameters);
        }

        public override MethodExpressionBase RootMethodExpression => _methodExpression;

        public override void FinaliseBody(Expression finalisedBody)
        {
            base.FinaliseBody(finalisedBody);
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
}