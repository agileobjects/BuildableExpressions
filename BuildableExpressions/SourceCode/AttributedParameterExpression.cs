namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class AttributedParameterExpression :
        Expression
    {
        private readonly ParameterExpression _parameterExpression;
        private readonly IList<AppliedAttribute> _attributes;

        public AttributedParameterExpression(
            ParameterExpression parameterExpression,
            IList<AppliedAttribute> attributes)
        {
            _parameterExpression = parameterExpression;
            _attributes = attributes;
        }

        public override ExpressionType NodeType => _parameterExpression.NodeType;

        public override Type Type => _parameterExpression.Type;

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            base.Accept(visitor);
            visitor.Visit(_parameterExpression);
            return this;
        }
    }
}