namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;
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
        /// Gets the Expression representing  the symbol to which the nameof operator is being applied.
        /// </summary>
        public Expression Operand { get; }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            var operandTranslation = context.GetTranslationFor(Operand);

            return new NameOfOperatorTranslation(operandTranslation, context);
        }
    }
}