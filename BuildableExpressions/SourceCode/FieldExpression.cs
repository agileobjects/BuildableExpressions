namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a field in a type in a piece of source code.
    /// </summary>
    public class FieldExpression :
        MemberExpression,
        IFieldExpressionConfigurator,
        IField
    {
        private readonly IType _type;

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
                SetVisibility(MemberVisibility.Public);
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

        /// <inheritdoc />
        public bool IsReadonly { get; private set; }

        #region IFieldExpressionConfigurator Members

        void IFieldExpressionConfigurator.SetStatic() => SetStatic();

        void IFieldExpressionConfigurator.SetReadonly() => IsReadonly = true;

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new FieldDefinitionTranslation(this, includeDeclaringType: false, context.Settings);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => GetFullTranslation(context);

        #endregion
    }
}