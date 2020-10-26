namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;

    internal class MethodExpressionScope : MethodScopeBase
    {
        private readonly MethodExpression _methodExpression;

        public MethodExpressionScope(MethodExpression methodExpression, MethodScopeBase parent)
            : base(parent)
        {
            _methodExpression = methodExpression;
            Add(methodExpression.Parameters);
        }

        public override MethodExpression GetOwningMethod() => _methodExpression;

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
}