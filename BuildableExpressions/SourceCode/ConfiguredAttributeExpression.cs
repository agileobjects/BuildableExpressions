namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    internal class ConfiguredAttributeExpression :
        AttributeExpression,
        IAttributeExpressionConfigurator
    {
        private static readonly AttributeExpression _attributeTypeExpression =
            TypeExpressionFactory.CreateAttribute(typeof(Attribute));

        private static readonly AttributeExpression _attributeUsageAttribute =
            TypeExpressionFactory.CreateAttribute(typeof(AttributeUsageAttribute));

        private AttributeTargets _targets;
        private Expression _baseInstanceExpression;

        public ConfiguredAttributeExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IAttributeExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            _targets = AttributeTargets.All;
            BaseTypeExpression = _attributeTypeExpression;

            configuration.Invoke(this);
            EnsureUsageAttribute();

            Validate();
        }

        #region Setup

        private void EnsureUsageAttribute()
        {
            if (AddUsageAttribute())
            {
                AddAttribute(
                    _attributeUsageAttribute,
                    attr => attr.SetConstructorArguments(ValidOn));
            }
        }

        private bool AddUsageAttribute()
        {
            var usageAttribute = AttributesAccessor?.FirstOrDefault(attr =>
                attr.AttributeExpression == _attributeUsageAttribute);

            if (usageAttribute == null)
            {
                return true;
            }

            if ((AttributeTargets)usageAttribute.Arguments.First().Value == ValidOn)
            {
                return false;
            }

            AttributesAccessor.Remove(usageAttribute);
            return true;
        }

        #endregion

        public override AttributeTargets ValidOn => _targets;

        #region IClassExpressionConfigurator Members

        void IClassBaseExpressionConfigurator.SetAbstract() => SetAbstract();

        internal void SetAbstract()
        {
            ThrowIfSealed("abstract");
            IsAbstract = true;
        }

        void IClassBaseExpressionConfigurator.SetSealed()
        {
            ThrowIfAbstract("sealed");
            IsSealed = true;
        }

        Expression IClassBaseExpressionConfigurator.BaseInstanceExpression
            => _baseInstanceExpression ??= InstanceExpression.Base(BaseTypeExpression);

        #endregion

        #region IAttributeExpressionConfigurator Members

        void IAttributeExpressionConfigurator.SetValidOn(AttributeTargets targets)
            => _targets = targets;

        void IAttributeExpressionConfigurator.SetBaseType(
            AttributeExpression baseAttributeExpression,
            Action<IAttributeImplementationConfigurator> configuration)
        {
            baseAttributeExpression.ThrowIfNull(nameof(baseAttributeExpression));
            ThrowIfBaseTypeAlreadySet(baseAttributeExpression);
            ThrowIfInvalidBaseType(baseAttributeExpression);

            if (configuration != null)
            {
                var configurator = new ImplementationConfigurator(
                    this,
                    baseAttributeExpression,
                    configuration);

                baseAttributeExpression = (AttributeExpression)configurator.ImplementedTypeExpression;
            }

            BaseTypeExpression = baseAttributeExpression;
        }

        private void ThrowIfBaseTypeAlreadySet(IType baseType)
        {
            if (BaseTypeExpression != _attributeTypeExpression)
            {
                ThrowBaseTypeAlreadySet(baseType, typeName: "attribute");
            }
        }

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}