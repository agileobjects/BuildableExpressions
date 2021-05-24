namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure how a <see cref="ClassExpression"/> implements a base type or
    /// interface.
    /// </summary>
    public interface IClassImplementationConfigurator :
        IImplementationConfigurator,
        IClassMemberConfigurator
    {
    }
}