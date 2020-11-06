namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents an interface in a piece of source code.
    /// </summary>
    public class InterfaceExpression :
        TypeExpression,
        IInterfaceExpressionConfigurator
    {
        internal InterfaceExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IInterfaceExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            configuration.Invoke(this);
        }

        #region IInterfaceExpressionConfigurator Members

        void IInterfaceExpressionConfigurator.SetImplements(
            Type @interface,
            Action<IImplementationConfigurator> configuration)
        {
            SetImplements(@interface, configuration);
        }

        PropertyExpression IInterfaceExpressionConfigurator.AddProperty(
            string name,
            Type type,
            Action<IInterfacePropertyExpressionConfigurator> configuration)
        {
            return AddProperty(new InterfacePropertyExpression(
                this,
                name,
                type,
                configuration));
        }

        MethodExpression IInterfaceExpressionConfigurator.AddMethod(
            string name,
            Type returnType,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return AddMethod(new InterfaceMethodExpression(
                this,
                name,
                returnType,
                configuration));
        }

        #endregion

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
            => new InterfaceTranslation(this, context);
    }
}