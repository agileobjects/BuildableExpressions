namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

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
        /// Gets the ExpressionType describing the type of this Expression - ExpressionType.Constant.
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => typeof(Type);

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
            return _hasOperandType
                ? new TypeofOperatorTranslation(OperandType, context.Settings)
                : new TypeofOperatorTranslation(OperandTypeName, context.Settings);
        }
    }
}