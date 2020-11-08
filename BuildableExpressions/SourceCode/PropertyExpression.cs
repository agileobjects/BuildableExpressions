namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    internal interface IHasSignature
    {
        string GetSignature();
    }

    internal interface IConcreteTypeExpression
    {
        bool IsAbstract { get; }
    }

    /// <summary>
    /// Represents a property in a type in a piece of source code.
    /// </summary>
    public class PropertyExpression :
        MemberExpression,
        IClassPropertyExpressionConfigurator,
        IProperty,
        IHasSignature,
        IConcreteTypeExpression
    {
        private IMember _getterMember;
        private IMember _setterMember;
        private PropertyInfo _propertyInfo;

        internal PropertyExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Type type,
            Action<PropertyExpression> configuration)
            : this(declaringTypeExpression, name, type)
        {
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(Public);
            }

            if (GetterExpression?.HasBody == false && SetterExpression == null)
            {
                SetSetter(s => s.SetVisibility(Private));
            }

            if (SetterExpression?.HasBody == false && GetterExpression == null)
            {
                SetGetter(s => s.SetVisibility(Private));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyExpression"/> class.
        /// </summary>
        /// <param name="declaringTypeExpression">
        /// This <see cref="PropertyExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </param>
        /// <param name="name">The name of this <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The type of this <see cref="PropertyExpression"/>.</param>
        protected PropertyExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Type type)
            : base(declaringTypeExpression, name)
        {
            Type = type;
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
            this.ValidateSetStatic();
            SetStatic();
        }

        void IClassPropertyExpressionConfigurator.SetAbstract()
        {
            this.ValidateSetAbstract();
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
            ThrowIfDuplicateAccessor(_getterMember);

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
            ThrowIfDuplicateAccessor(_setterMember);

            _setterMember = SetterExpression = new PropertyAccessorExpression(this, isGetter: false);
            configuration.Invoke(SetterExpression);
        }

        private void ThrowIfDuplicateAccessor(IMember accessorMember)
        {
            if (accessorMember == null)
            {
                return;
            }

            var accessor = accessorMember == _getterMember ? "get" : "set";

            throw new InvalidOperationException(
                $"Property '{GetSignature()}' has multiple {accessor} accessors configured.");
        }

        #endregion

        string IHasSignature.GetSignature() => GetSignature();

        private string GetSignature() => Type.GetFriendlyName() + " " + Name;

        #region IProperty Members

        bool IComplexMember.IsVirtual { get; }

        bool IComplexMember.IsOverride { get; }

        bool IProperty.IsReadable => _getterMember?.IsPublic == true;

        IComplexMember IProperty.Getter => GetterExpression;

        bool IProperty.IsWritable => _setterMember?.IsPublic == true;

        IComplexMember IProperty.Setter => SetterExpression;

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new PropertyTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => new TransientPropertyTranslation(this, context);

        #endregion
    }
}