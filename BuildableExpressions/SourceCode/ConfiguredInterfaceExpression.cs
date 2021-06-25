namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    internal class ConfiguredInterfaceExpression :
        InterfaceExpression,
        IInterfaceExpressionConfigurator
    {
        private readonly ConfiguredSourceCodeExpression _sourceCode;

        public ConfiguredInterfaceExpression(
            ConfiguredSourceCodeExpression sourceCode,
            string name,
            Action<IInterfaceExpressionConfigurator> configuration)
            : this(sourceCode, name)
        {
            _sourceCode = sourceCode;
            configuration?.Invoke(this);
        }

        private ConfiguredInterfaceExpression(ConfiguredSourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IClosableTypeExpression Members

        protected override InterfaceExpression CreateInstance()
            => new ConfiguredInterfaceExpression(_sourceCode, Name);

        #endregion

        #region IInterfaceExpressionConfigurator Members

        void IInterfaceExpressionConfigurator.SetImplements(
            InterfaceExpression interfaceExpression,
            Action<IImplementationConfigurator> configuration)
        {
            SetImplements(interfaceExpression, configuration);
        }

        PropertyExpression IInterfaceExpressionConfigurator.AddProperty(
            string name,
            IType type,
            Action<IInterfacePropertyExpressionConfigurator> configuration)
        {
            return AddProperty(
                new InterfacePropertyExpression(this, name, type, configuration));
        }

        MethodExpression IInterfaceExpressionConfigurator.AddMethod(string name,
            IType returnType,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return AddMethod(new ConfiguredInterfaceMethodExpression(
                this,
                name,
                returnType,
                configuration));
        }

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new InterfaceTranslation(this, context);
    }
}