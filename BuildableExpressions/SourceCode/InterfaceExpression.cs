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

        MethodExpression IInterfaceExpressionConfigurator.AddMethod(
            string name,
            Type returnType,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return Add(new InterfaceMethodExpression(
                this,
                name,
                returnType,
                configuration));
        }

        #endregion

        internal override ITranslation GetTranslation(ITranslationContext context)
            => new InterfaceTranslation(this, context);
    }
}