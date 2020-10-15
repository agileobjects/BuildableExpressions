namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;

    /// <summary>
    /// Represents a use of the default operator.
    /// </summary>
    public sealed class DefaultOperatorExpression :
        Expression,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        internal DefaultOperatorExpression(Expression operand)
        {
            Operand = operand;
        }

        /// <summary>
        /// Gets the ExpressionType describing the type of this Expression - ExpressionType.Default.
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Default;

        /// <inheritdoc />
        public override Type Type => Operand.Type;

        /// <summary>
        /// Visits this <see cref="DefaultOperatorExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="DefaultOperatorExpression"/>.
        /// </param>
        /// <returns>This <see cref="DefaultOperatorExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets the Expression representing the symbol to which the nameof operator is being applied.
        /// </summary>
        public Expression Operand { get; }

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
        {
            get { yield return Operand; }
        }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            var operandTranslation = context.GetTranslationFor(Operand);

            return new DefaultOperatorTranslation(operandTranslation, context.Settings);
        }
    }
}