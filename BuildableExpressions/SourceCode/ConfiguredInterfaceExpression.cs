namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;

    internal class ConfiguredInterfaceExpression :
        InterfaceExpression,
        IInterfaceExpressionConfigurator
    {
        public ConfiguredInterfaceExpression(
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
    }
}