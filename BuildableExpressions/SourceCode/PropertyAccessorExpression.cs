namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

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
        public override bool IsStatic => _property.IsStatic;

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
}