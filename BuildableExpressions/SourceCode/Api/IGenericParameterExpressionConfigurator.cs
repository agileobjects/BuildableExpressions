namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="GenericParameterExpression"/>.
    /// </summary>
    public interface IGenericParameterExpressionConfigurator
    {
        /// <summary>
        /// Set the name of the <see cref="GenericParameterExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator Named(string name);
    }
}