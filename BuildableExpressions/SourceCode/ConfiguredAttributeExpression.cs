namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using Translations;

    internal class ConfiguredAttributeExpression :
        AttributeExpression,
        IAttributeExpressionConfigurator
    {
        public ConfiguredAttributeExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IAttributeExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            configuration.Invoke(this);
            Validate();
        }

        #region IAttributeExpressionConfigurator Members

        void IAttributeExpressionConfigurator.SetBaseType(
            AttributeExpression baseAttributeExpression, 
            Action<IAttributeImplementationConfigurator> configuration)
        {
            baseAttributeExpression.ThrowIfNull(nameof(baseAttributeExpression));
            //ThrowIfBaseTypeAlreadySet(baseAttributeExpression);
            //ThrowIfInvalidBaseType(baseAttributeExpression);

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

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new AttributeTranslation(this, context);
    }
}