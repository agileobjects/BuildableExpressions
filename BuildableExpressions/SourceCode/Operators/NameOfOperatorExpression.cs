namespace AgileObjects.BuildableExpressions.SourceCode.Operators
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    /// <summary>
    /// Represents a use of the nameof operator.
    /// </summary>
    public sealed class NameOfOperatorExpression :
        Expression,
        ICustomTranslationExpression
    {
        internal NameOfOperatorExpression(Expression operand)
        {
            Operand = operand;
        }

        /// <summary>
        /// Gets the ExpressionType describing the type of this Expression - ExpressionType.Extension.
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => typeof(string);

        /// <summary>
        /// Visits this <see cref="NameOfOperatorExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="NameOfOperatorExpression"/>.
        /// </param>
        /// <returns>This <see cref="NameOfOperatorExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets the Expression representing the symbol to which the nameof operator is being applied.
        /// </summary>
        public Expression Operand { get; }

        /// <summary>
        /// Returns a <see cref="NameOfOperatorExpression"/> for the given <paramref name="operand"/>,
        /// or this instance if the <paramref name="operand"/> is the same object as
        /// <see cref="Operand"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="NameOfOperatorExpression"/> for the given <paramref name="operand"/>,
        /// or this instance if the <paramref name="operand"/> is the same object as
        /// <see cref="Operand"/>.
        /// </returns>
        public NameOfOperatorExpression Update(Expression operand)
            => operand != Operand ? new NameOfOperatorExpression(operand) : this;

        #region ICustomTranslationExpression Members

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            var operandTranslation = context.GetTranslationFor(Operand);

            return new NameOfOperatorTranslation(operandTranslation, context);
        }

        #endregion
    }
}