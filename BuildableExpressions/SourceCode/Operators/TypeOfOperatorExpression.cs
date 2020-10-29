namespace AgileObjects.BuildableExpressions.SourceCode.Operators
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Formatting;

    /// <summary>
    /// Represents a use of the typeof operator.
    /// </summary>
    public sealed class TypeOfOperatorExpression :
        Expression,
        ICustomTranslationExpression
    {
        private readonly bool _hasOperandType;
        private string _typeName;

        internal TypeOfOperatorExpression(string typeName)
        {
            OperandType = typeof(void);
            _typeName = typeName;
        }

        internal TypeOfOperatorExpression(Type type)
        {
            OperandType = type;
            _hasOperandType = true;
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
        /// Gets the type to which the typeof operator is being applied. If this Expression represents
        /// a typeof operation on an open generic argument type, returns void.
        /// </summary>
        public Type OperandType { get; }

        /// <summary>
        /// Gets the name of the type to which the typeof operator is being applied.
        /// </summary>
        public string OperandTypeName
            => _typeName ??= Type.GetFriendlyName();

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            var operandTranslation = _hasOperandType
                ? (ITranslatable)context.GetTranslationFor(OperandType)
                : new FixedValueTranslation(
                    NodeType,
                    OperandTypeName,
                    Type,
                    TokenType.TypeName,
                    context);

            return new TypeOfOperatorTranslation(operandTranslation, context.Settings);
        }
    }
}