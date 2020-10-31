namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a property of field in a type in a piece of source code.
    /// </summary>
    public class PropertyOrFieldExpression :
        MemberExpression,
        IClassPropertyExpressionConfigurator,
        IProperty
    {
        private PropertyAccessor _getter;
        private IMember _getterMember;
        private PropertyAccessor _setter;
        private IMember _setterMember;

        internal PropertyOrFieldExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Type type,
            Action<PropertyOrFieldExpression> configuration)
            : base(declaringTypeExpression, name)
        {
            Type = type;
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(MemberVisibility.Public);
            }
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1005) indicating the type of this
        /// <see cref="PropertyOrFieldExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Property;

        /// <summary>
        /// Gets the type of this <see cref="PropertyOrFieldExpression"/>.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Visits this <see cref="PropertyOrFieldExpression"/>.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="PropertyOrFieldExpression"/>.</param>
        /// <returns>This <see cref="PropertyOrFieldExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyOrFieldExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

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
            _getterMember = _getter = new PropertyAccessor(this, isGetter: true);
            configuration.Invoke(_getter);
        }

        void IConcreteTypePropertyExpressionConfigurator.SetSetter(
            Action<IPropertySetterConfigurator> configuration)
        {
            _setterMember = _setter = new PropertyAccessor(this, isGetter: false);
            configuration.Invoke(_setter);
        }

        #endregion

        #region IProperty Members

        bool IComplexMember.IsVirtual { get; }

        bool IComplexMember.IsOverride { get; }

        bool IProperty.IsReadable => _getterMember?.IsPublic == true;

        IComplexMember IProperty.Getter => _getter;

        bool IProperty.IsWritable => _setterMember?.IsPublic == true;

        IComplexMember IProperty.Setter => _setter;

        #endregion

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
        {
            return new PropertyDefinitionTranslation(
                this,
                includeDeclaringType: false,
                context.Settings);
        }

        #region Accessor Class

        private class PropertyAccessor :
            MemberExpressionBase,
            IComplexMember,
            IPropertyGetterConfigurator,
            IPropertySetterConfigurator
        {
            private readonly IProperty _property;

            public PropertyAccessor(IProperty property, bool isGetter)
                : base(isGetter ? "get" : "set")
            {
                _property = property;
                Type = isGetter ? property.Type : typeof(void);
            }

            public override Type Type { get; }

            public override Type DeclaringType => _property.DeclaringType;

            public bool IsAbstract => _property.IsAbstract;

            public bool IsVirtual => _property.IsVirtual;

            public bool IsOverride => _property.IsOverride;

            public Expression Body { get; private set; }

            public void SetVisibility(MemberVisibility visibility)
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
        }

        #endregion
    }
}