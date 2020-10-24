namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/> or <see cref="MethodExpression"/>'s
    /// generic parameters.
    /// </summary>
    public interface IGenericParameterConfigurator
    {
        /// <summary>
        /// Adds a <see cref="GenericParameterExpression"/> to the <see cref="TypeExpression"/> or
        /// <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <param name="configuration">The configuration to use for the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>The newly-created <see cref="GenericParameterExpression"/>.</returns>
        GenericParameterExpression AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration);
    }
}