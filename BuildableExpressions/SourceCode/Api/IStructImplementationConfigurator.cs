namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure how a <see cref="StructExpression"/> implements an interface.
    /// </summary>
    public interface IStructImplementationConfigurator :
        IImplementationConfigurator,
        IStructMethodConfigurator
    {
    }
}