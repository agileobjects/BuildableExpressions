namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Represents a property in a type in a piece of source code.
    /// </summary>
    public class PropertyExpression :
        MemberExpression,
        IClassPropertyExpressionConfigurator,
        IProperty
    {
        private IMember _getterMember;
        private IMember _setterMember;
        private PropertyInfo _propertyInfo;

        internal PropertyExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Type type,
            Action<PropertyExpression> configuration)
            : base(declaringTypeExpression, name)
        {
            Type = type;
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(Public);
            }
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1005) indicating the type of this
        /// <see cref="PropertyExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Property;

        /// <summary>
        /// Gets the type of this <see cref="PropertyExpression"/>.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Visits this <see cref="PropertyExpression"/>.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="PropertyExpression"/>.</param>
        /// <returns>This <see cref="PropertyExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyExpression"/> represents an
        /// auto-property.
        /// </summary>
        public virtual bool IsAutoProperty
            => GetterExpression?.HasBody != true && SetterExpression?.HasBody != true;

        /// <summary>
        /// Gets a <see cref="PropertyAccessorExpression"/> describing this
        /// <see cref="PropertyExpression"/>'s get accessor, or null if none exists.
        /// </summary>
        public PropertyAccessorExpression GetterExpression { get; private set; }

        /// <summary>
        /// Gets a <see cref="PropertyAccessorExpression"/> describing this
        /// <see cref="PropertyExpression"/>'s set accessor, or null if none exists.
        /// </summary>
        public PropertyAccessorExpression SetterExpression { get; private set; }

        /// <summary>
        /// Gets the PropertyInfo for this <see cref="PropertyExpression"/>, which is lazily,
        /// dynamically generated using this property's definition.
        /// </summary>
        public PropertyInfo PropertyInfo
            => _propertyInfo ??= CreatePropertyInfo();

        #region PropertyInfo Creation

        private PropertyInfo CreatePropertyInfo()
        {
            var property = Visibility == Public
                ? IsStatic
                    ? DeclaringType.GetPublicStaticProperty(Name)
                    : DeclaringType.GetPublicInstanceProperty(Name)
                : IsStatic
                    ? DeclaringType.GetNonPublicStaticProperty(Name)
                    : DeclaringType.GetNonPublicInstanceProperty(Name);

            return property;
        }

        #endregion

        #region IClassPropertyExpressionConfigurator Members

        void IPropertyExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        void IConcreteTypePropertyExpressionConfigurator.SetStatic()
        {
            SetStatic();
        }

        void IClassPropertyExpressionConfigurator.SetAbstract()
        {
            SetAbstract();
        }

        /// <summary>
        /// Mark this <see cref="PropertyExpression"/> as abstract.
        /// </summary>
        protected void SetAbstract() => IsAbstract = true;

        void IConcreteTypePropertyExpressionConfigurator.SetGetter(
            Action<IPropertyGetterConfigurator> configuration)
        {
            SetGetter(configuration);
        }

        /// <summary>
        /// Add a getter to this <see cref="PropertyExpression"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        protected void SetGetter(Action<PropertyAccessorExpression> configuration)
        {
            _getterMember = GetterExpression = new PropertyAccessorExpression(this, isGetter: true);
            configuration.Invoke(GetterExpression);
        }

        void IConcreteTypePropertyExpressionConfigurator.SetSetter(
            Action<IPropertySetterConfigurator> configuration)
        {
            SetSetter(configuration);
        }

        /// <summary>
        /// Add a setter to this <see cref="PropertyExpression"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        protected void SetSetter(Action<PropertyAccessorExpression> configuration)
        {
            _setterMember = SetterExpression = new PropertyAccessorExpression(this, isGetter: false);
            configuration.Invoke(SetterExpression);
        }

        #endregion

        #region IProperty Members

        bool IComplexMember.IsVirtual { get; }

        bool IComplexMember.IsOverride { get; }

        bool IProperty.IsReadable => _getterMember?.IsPublic == true;

        IComplexMember IProperty.Getter => GetterExpression;

        bool IProperty.IsWritable => _setterMember?.IsPublic == true;

        IComplexMember IProperty.Setter => SetterExpression;

        #endregion

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
            => new PropertyTranslation(this, context);
    }
}