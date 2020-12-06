namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Represents a field in a type in a piece of source code.
    /// </summary>
    public class FieldExpression :
        MemberExpression,
        IFieldExpressionConfigurator,
        IField
    {
        private readonly IType _type;
        private FieldInfo _fieldInfo;

        internal FieldExpression(
            TypeExpression declaringTypeExpression,
            string name,
            IType type,
            Action<FieldExpression> configuration)
            : base(declaringTypeExpression, name)
        {
            _type = type;
            configuration.Invoke(this);

            if (Visibility == default)
            {
                SetVisibility(Public);
            }
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1006) indicating the type of this
        /// <see cref="FieldExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Field;

        /// <summary>
        /// Gets the type of this <see cref="FieldExpression"/>.
        /// </summary>
        public override Type Type => _type.AsType();

        /// <summary>
        /// Visits this <see cref="FieldExpression"/>.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="FieldExpression"/>.</param>
        /// <returns>This <see cref="FieldExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConstant { get; private set; }

        /// <inheritdoc />
        public bool IsReadonly { get; private set; }

        /// <summary>
        /// Gets an Expression representing this <see cref="FieldExpression"/>'s initial value.
        /// </summary>
        public Expression InitialValue { get; private set; }

        /// <summary>
        /// Gets the FieldInfo for this <see cref="PropertyExpression"/>, which is lazily,
        /// dynamically generated using this field's definition.
        /// </summary>
        public FieldInfo FieldInfo
            => _fieldInfo ??= CreateFieldInfo();

        #region FieldInfo Creation

        private FieldInfo CreateFieldInfo()
        {
            var declaringType = DeclaringType.AsType();

            var field = Visibility == Public
                ? IsStatic
                    ? declaringType.GetPublicStaticField(Name)
                    : declaringType.GetPublicInstanceField(Name)
                : IsStatic
                    ? declaringType.GetNonPublicStaticField(Name)
                    : declaringType.GetNonPublicInstanceField(Name);

            return field;
        }

        #endregion

        #region IFieldExpressionConfigurator Members

        void IFieldExpressionConfigurator.SetStatic() => SetStatic();

        void IFieldExpressionConfigurator.SetConstant() => IsConstant = IsReadonly = true;

        void IFieldExpressionConfigurator.SetReadonly() => IsReadonly = true;

        void IFieldExpressionConfigurator.SetInitialValue(Expression value)
            => InitialValue = value;

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new FieldTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context) 
            => GetFullTranslation(context);

        #endregion

        /// <inheritdoc />
        public override string ToString() => $"{_type.GetFriendlyName()} {Name}";
    }
}