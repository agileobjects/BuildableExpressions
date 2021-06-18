namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using Extensions;

    /// <summary>
    /// Abstract base class for types to which <see cref="AttributeExpression"/>s can be applied.
    /// </summary>
    public abstract class AttributableExpressionBase :
        Expression,
        IAttributableExpressionConfigurator
    {
        private List<AppliedAttribute> _attributes;
        private ReadOnlyCollection<AppliedAttribute> _readOnlyAttributes;

        /// <summary>
        /// Gets the <see cref="AppliedAttribute"/>s describing the
        /// <see cref="AttributeExpression"/>s which have been applied to this
        /// <see cref="AttributableExpressionBase"/>, if any.
        /// </summary>
        public ReadOnlyCollection<AppliedAttribute> Attributes
            => _readOnlyAttributes ??= _attributes.ToReadOnlyCollection();

        internal IList<AppliedAttribute> AttributesAccessor => _attributes;

        #region IAttributableExpressionConfigurator Members

        void IAttributableExpressionConfigurator.AddAttribute<TAttribute>() 
            => AddAttribute(typeof(TAttribute), configuration: null);

        void IAttributableExpressionConfigurator.AddAttribute<TAttribute>(
            Action<IAttributeApplicationConfigurator> configuration)
        {
            AddAttribute(typeof(TAttribute), configuration);
        }

        void IAttributableExpressionConfigurator.AddAttribute(Type attributeType) 
            => AddAttribute(attributeType, configuration: null);

        void IAttributableExpressionConfigurator.AddAttribute(
            Type attributeType,
            Action<IAttributeApplicationConfigurator> configuration)
        {
            AddAttribute(attributeType, configuration);
        }

        private void AddAttribute(
            Type attributeType,
            Action<IAttributeApplicationConfigurator> configuration)
        {
            AddAttribute(TypeExpressionFactory.CreateAttribute(attributeType), configuration);
        }

        void IAttributableExpressionConfigurator.AddAttribute(AttributeExpression attribute)
            => AddAttribute(attribute, configuration: null);

        AppliedAttribute IAttributableExpressionConfigurator.AddAttribute(
            AttributeExpression attribute,
            Action<IAttributeApplicationConfigurator> configuration)
        {
            return AddAttribute(attribute, configuration);
        }

        /// <summary>
        /// Applies the given <paramref name="attribute"/> to this
        /// <see cref="AttributableExpressionBase"/>.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="AttributeExpression"/> to apply to this
        /// <see cref="AttributableExpressionBase"/>.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>
        /// The <see cref="AppliedAttribute"/> describing the application of the given
        /// <paramref name="attribute"/>.
        /// </returns>
        protected AppliedAttribute AddAttribute(
            AttributeExpression attribute,
            Action<IAttributeApplicationConfigurator> configuration)
        {
            var appliedAttribute = new AppliedAttribute(attribute);
            configuration?.Invoke(appliedAttribute);

            _attributes ??= new List<AppliedAttribute>();
            _attributes.Add(appliedAttribute);
            _readOnlyAttributes = null;

            return appliedAttribute;
        }

        #endregion
    }
}