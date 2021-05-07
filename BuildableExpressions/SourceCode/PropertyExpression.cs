namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Represents a property in a type in a piece of source code.
    /// </summary>
    public abstract class PropertyExpression :
        MemberExpression,
        IProperty,
        IHasSignature
    {
        private IMember _getterMember;
        private IMember _setterMember;
        private PropertyInfo _propertyInfo;
        private IType _type;

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
            IType type)
            : base(declaringTypeExpression, name)
        {
            _type = type;
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1007) indicating the type of this
        /// <see cref="PropertyExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Property;

        /// <summary>
        /// Gets the type of this <see cref="PropertyExpression"/>.
        /// </summary>
        public override Type Type => _type.AsType();

        /// <summary>
        /// Visits this <see cref="PropertyExpression"/>.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="PropertyExpression"/>.</param>
        /// <returns>This <see cref="PropertyExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            base.Accept(visitor);
            visitor.Visit(GetterExpression);
            visitor.Visit(SetterExpression);
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyExpression"/> is virtual.
        /// </summary>
        public bool IsVirtual { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyExpression"/> overrides a base
        /// type property.
        /// </summary>
        public abstract bool IsOverride { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyExpression"/> represents an
        /// auto-property.
        /// </summary>
        public abstract bool IsAutoProperty { get; }

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
            var declaringType = DeclaringType.AsType();

            var property = Visibility == Public
                ? IsStatic
                    ? declaringType.GetPublicStaticProperty(Name)
                    : declaringType.GetPublicInstanceProperty(Name)
                : IsStatic
                    ? declaringType.GetNonPublicStaticProperty(Name)
                    : declaringType.GetNonPublicInstanceProperty(Name);

            return property;
        }

        #endregion

        string IHasSignature.GetSignature() => GetSignature();

        private string GetSignature() => Type.GetFriendlyName() + " " + Name;

        /// <summary>
        /// Mark this <see cref="PropertyExpression"/> as abstract.
        /// </summary>
        protected void SetAbstract()
        {
            IsAbstract = true;
            SetVirtual();
        }

        /// <summary>
        /// Mark this <see cref="PropertyExpression"/> as virtual.
        /// </summary>
        protected void SetVirtual() => IsVirtual = true;

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

            ThrowIfInvalidVisibility(GetterExpression);
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

            ThrowIfInvalidVisibility(SetterExpression);
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

        private void ThrowIfInvalidVisibility(PropertyAccessorExpression accessorExpression)
        {
            if ((accessorExpression.Visibility ?? Public) >= (Visibility ?? Public))
            {
                return;
            }

            var visibility = this.GetAccessibility();
            var accessor = accessorExpression.Name;
            var accessorVisibility = accessorExpression.GetAccessibility();

            throw new InvalidOperationException(
                $"Property '{GetSignature()}' with visibility '{visibility}' " +
                $"cannot have a {accessor} accessor with " +
                $"less restrictive visibility '{accessorVisibility}'");
        }

        #region IMember Members

        IType IMember.DeclaringType => DeclaringTypeExpression;

        IType IMember.Type => _type ??= BclTypeWrapper.For(Type);

        #endregion

        #region IComplexMember Members

        bool IComplexMember.IsOverride => IsOverride;

        #endregion

        #region IProperty Members

        bool IProperty.IsReadable => _getterMember?.IsPublic == true;

        IMethod IProperty.Getter => GetterExpression;

        bool IProperty.IsWritable => _setterMember?.IsPublic == true;

        IMethod IProperty.Setter => SetterExpression;

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new PropertyTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => new ReturnDefaultPropertyTranslation(this, context);

        #endregion

        internal override void ResetMemberInfo() => _propertyInfo = null;

        /// <inheritdoc />
        public override string ToString() => $"{_type.GetFriendlyName()} {Name}";
    }
}