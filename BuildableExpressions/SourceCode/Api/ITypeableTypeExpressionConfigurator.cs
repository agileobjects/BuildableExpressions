namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using Generics;

    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/> with optional generic
    /// parameters.
    /// </summary>
    public interface ITypeableTypeExpressionConfigurator :
        ITypeExpressionConfigurator,
        IGenericParameterConfigurator
    {
        /// <summary>
        /// Adds the given open generic <paramref name="parameter"/> to the
        /// <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        void AddGenericParameter(GenericParameterExpression parameter);
    }
}