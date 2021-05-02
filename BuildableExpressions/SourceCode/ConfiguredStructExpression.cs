namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    internal class ConfiguredStructExpression :
        StructExpression,
        IStructExpressionConfigurator
    {
        public ConfiguredStructExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IStructExpressionConfigurator> configuration)
            : this(sourceCode, name)
        {
            configuration.Invoke(this);
            Validate();
        }

        private ConfiguredStructExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IStructExpressionConfigurator Members

        void IStructExpressionConfigurator.SetImplements(
            InterfaceExpression interfaceExpression,
            Action<IStructImplementationConfigurator> configuration)
        {
            SetImplements(interfaceExpression, configuration);
        }

        PropertyExpression IStructMemberConfigurator.AddProperty(
            string name,
            IType type,
            Action<IConcreteTypePropertyExpressionConfigurator> configuration)
        {
            return AddProperty(name, type, configuration);
        }

        MethodExpression IStructMemberConfigurator.AddMethod(
            string name,
            Action<IConcreteTypeMethodExpressionConfigurator> configuration)
        {
            return AddMethod(name, configuration);
        }

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new StructTranslation(this, context);
    }
}