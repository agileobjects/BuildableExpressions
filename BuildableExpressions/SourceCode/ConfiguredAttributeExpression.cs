namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
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

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new AttributeTranslation(this, context);
    }
}