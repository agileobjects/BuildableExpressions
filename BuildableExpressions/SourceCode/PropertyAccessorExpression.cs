namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a property accessor in a piece of source code.
    /// </summary>
    public class PropertyAccessorExpression :
        MemberExpressionBase,
        IMethod,
        IPropertyGetterConfigurator,
        IPropertySetterConfigurator,
        ICustomTranslationExpression
    {
        private readonly IProperty _property;
        private readonly IType _type;
        private Type _bclType;

        internal PropertyAccessorExpression(IProperty property, bool isGetter)
            : base(isGetter ? "get" : "set")
        {
            _property = property;
            _type = isGetter ? property.Type : BclTypeWrapper.Void;
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1008) indicating the type of this
        /// <see cref="PropertyAccessorExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.PropertyAccessor;

        /// <summary>
        /// Gets the type of this <see cref="PropertyAccessorExpression"/>.
        /// </summary>
        public override Type Type => _bclType ??= _type.AsType();

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
        public override IType DeclaringType => _property.DeclaringType;

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

        #region IPropertyGetterConfigurator Members

        void IPropertyGetterConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        void IPropertySetterConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        internal new void SetVisibility(MemberVisibility visibility)
            => base.SetVisibility(visibility);

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

        #endregion

        #region IMethod Members

        bool IMethod.IsGenericMethod => false;

        bool IMethodBase.IsExtensionMethod => false;

        IMethod IMethod.GetGenericMethodDefinition() => null;

        ReadOnlyCollection<IGenericParameter> IMethod.GetGenericArguments()
            => Enumerable<IGenericParameter>.EmptyReadOnlyCollection;

        ReadOnlyCollection<IParameter> IMethodBase.GetParameters()
            => Enumerable<IParameter>.EmptyReadOnlyCollection;

        IType IMethod.ReturnType => _type;

        #endregion

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => context.GetTranslationFor(Body);
    }
}