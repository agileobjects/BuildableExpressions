namespace AgileObjects.BuildableExpressions.SourceCode.Operators
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;

    /// <summary>
    /// Represents a use of the typeof operator.
    /// </summary>
    public sealed class TypeOfOperatorExpression :
        Expression,
        ICustomTranslationExpression
    {
        private readonly TypeExpression _typeExpression;

        internal TypeOfOperatorExpression(TypeExpression typeExpression)
        {
            _typeExpression = typeExpression;
        }

        /// <summary>
        /// Gets the ExpressionType describing the type of this Expression - ExpressionType.Extension.
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => typeof(Type);

        /// <summary>
        /// Visits this <see cref="TypeOfOperatorExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="TypeOfOperatorExpression"/>.
        /// </param>
        /// <returns>This <see cref="TypeOfOperatorExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets the type to which the typeof operator is being applied.
        /// </summary>
        public Type Operand => _typeExpression.Type;

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new TypeOfOperatorTranslation(_typeExpression, context);
    }
}