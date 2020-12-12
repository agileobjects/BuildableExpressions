namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;

    internal class BlockMethodCallExpression :
        Expression,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        private readonly BlockMethodExpression _blockMethod;

        public BlockMethodCallExpression(BlockMethodExpression blockMethod)
        {
            _blockMethod = blockMethod;
        }

        public override ExpressionType NodeType => ExpressionType.Call;

        public override Type Type => _blockMethod.Type;

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(_blockMethod);
            return this;
        }

        #region ICustomAnalysableExpression Members

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
        {
            get { yield return _blockMethod; }
        }

        #endregion

        #region ICustomTranslationExpression Members

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => MethodCallTranslation.For(_blockMethod, _blockMethod.Parameters, context);

        #endregion
    }
}