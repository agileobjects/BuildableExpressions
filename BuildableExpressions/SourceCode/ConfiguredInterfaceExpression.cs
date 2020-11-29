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
        public ConfiguredInterfaceExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IInterfaceExpressionConfigurator> configuration)
            : this(sourceCode, name)
        {
            configuration.Invoke(this);
        }

        private ConfiguredInterfaceExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

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
            return AddProperty(new InterfacePropertyExpression(
                this,
                name,
                type,
                configuration));
        }

        MethodExpression IInterfaceExpressionConfigurator.AddMethod(string name,
            IType returnType,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return AddMethod(new InterfaceMethodExpression(
                this,
                name,
                returnType,
                configuration));
        }

        #endregion

        protected override TypeExpression CreateInstance()
            => new ConfiguredInterfaceExpression(SourceCode, Name);

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new InterfaceTranslation(this, context);
    }
}