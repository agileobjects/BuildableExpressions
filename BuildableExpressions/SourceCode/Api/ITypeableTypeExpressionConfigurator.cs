namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/> with optional generic parameters.
    /// </summary>
    public interface ITypeableTypeExpressionConfigurator :
        ITypeExpressionConfigurator,
        IGenericParameterConfigurator
    {
    }
}