namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using Translations;

    internal class ConfiguredAttributeExpression :
        AttributeExpression,
        IAttributeExpressionConfigurator
    {
        private Expression _baseInstanceExpression;

        public ConfiguredAttributeExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IAttributeExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            BaseTypeExpression = TypeExpressionFactory.CreateAttribute(typeof(Attribute));
            configuration.Invoke(this);
            Validate();
        }

        #region IClassExpressionConfigurator Members

        void IClassBaseExpressionConfigurator.SetAbstract() => SetAbstract();

        internal void SetAbstract()
        {
            //ThrowIfSealed("abstract");
            IsAbstract = true;
        }

        void IClassBaseExpressionConfigurator.SetSealed()
        {
            //ThrowIfAbstract("sealed");
            IsSealed = true;
        }

        Expression IClassBaseExpressionConfigurator.BaseInstanceExpression
            => _baseInstanceExpression ??= InstanceExpression.Base(BaseTypeExpression);

        #endregion

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
            => new ClassTranslation(this, context);
    }
}