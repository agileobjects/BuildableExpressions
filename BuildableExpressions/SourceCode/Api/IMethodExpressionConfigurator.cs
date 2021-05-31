namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/>.
    /// </summary>
    public interface IMethodExpressionConfigurator :
        IGenericParameterConfigurator,
        IMethodExpressionBaseConfigurator
    {
    }
}