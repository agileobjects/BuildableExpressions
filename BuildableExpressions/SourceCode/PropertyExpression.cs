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
        public bool IsAutoProperty
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
            var declaringType = DeclaringTypeExpression.Type;

            var property = Visibility == Public
                ? declaringType.GetPublicInstanceProperty(Name)
                : declaringType.GetNonPublicInstanceProperty(Name);

            return property;
        }

        #endregion

        #region IClassPropertyExpressionConfigurator Members

        void IPropertyExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        private void SetVisibility(MemberVisibility visibility)
            => Visibility = visibility;

        void IConcreteTypePropertyExpressionConfigurator.SetStatic()
        {
            IsStatic = true;
        }

        void IClassPropertyExpressionConfigurator.SetAbstract()
        {
            IsAbstract = true;
        }

        void IConcreteTypePropertyExpressionConfigurator.SetGetter(
            Action<IPropertyGetterConfigurator> configuration)
        {
            _getterMember = GetterExpression = new PropertyAccessorExpression(this, isGetter: true);
            configuration.Invoke(GetterExpression);
        }

        void IConcreteTypePropertyExpressionConfigurator.SetSetter(
            Action<IPropertySetterConfigurator> configuration)
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

        #region Accessor Class

        /// <summary>
        /// Represents a property accessor in a piece of source code.
        /// </summary>
        public class PropertyAccessorExpression :
            MemberExpressionBase,
            IComplexMember,
            IPropertyGetterConfigurator,
            IPropertySetterConfigurator,
            ICustomTranslationExpression
        {
            private readonly IProperty _property;

            internal PropertyAccessorExpression(IProperty property, bool isGetter)
                : base(isGetter ? "get" : "set")
            {
                _property = property;
                Type = isGetter ? property.Type : typeof(void);
            }

            /// <summary>
            /// Gets the <see cref="SourceCodeExpressionType"/> value (1006) indicating the type of this
            /// <see cref="PropertyAccessorExpression"/> as an ExpressionType.
            /// </summary>
            public override ExpressionType NodeType
                => (ExpressionType)SourceCodeExpressionType.PropertyAccessor;

            /// <summary>
            /// Gets the type of this <see cref="PropertyAccessorExpression"/>.
            /// </summary>
            public override Type Type { get; }

            /// <summary>
            /// Visits this <see cref="PropertyAccessorExpression"/>.
            /// </summary>
            /// <param name="visitor">
            /// The visitor with which to visit this <see cref="PropertyAccessorExpression"/>.
            /// </param>
            /// <returns>This <see cref="PropertyAccessorExpression"/>.</returns>
            protected override Expression Accept(ExpressionVisitor visitor)
            {
                return this;
            }

            /// <inheritdoc />
            public override Type DeclaringType => _property.DeclaringType;

            /// <inheritdoc />
            public bool IsAbstract => _property.IsAbstract;

            /// <inheritdoc />
            public bool IsVirtual => _property.IsVirtual;

            /// <inheritdoc />
            public bool IsOverride => _property.IsOverride;

            /// <summary>
            /// Gets a value indicating whether this <see cref="PropertyAccessorExpression"/> has
            /// a body.
            /// </summary>
            public bool HasBody => Body != null;

            /// <summary>
            /// Gets an Expression describing this <see cref="PropertyAccessorExpression"/>'s body,
            /// or null if the parent <see cref="PropertyExpression"/> is an auto-property.
            /// </summary>
            public Expression Body { get; private set; }

            void IPropertyGetterConfigurator.SetVisibility(MemberVisibility visibility)
                => SetVisibility(visibility);

            void IPropertySetterConfigurator.SetVisibility(MemberVisibility visibility)
                => SetVisibility(visibility);

            internal void SetVisibility(MemberVisibility visibility)
            {
                Visibility = visibility;
            }

            void IPropertyGetterConfigurator.SetBody(Expression body)
            {
                Body = body;
            }

            void IPropertySetterConfigurator.SetBody(
                Func<ParameterExpression, Expression> bodyFactory)
            {
                var valueVariable = Variable(Type, "value");

                Body = Block(
                    new[] { valueVariable },
                    bodyFactory.Invoke(valueVariable));
            }

            ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
                => context.GetTranslationFor(Body);
        }

        #endregion
    }
}