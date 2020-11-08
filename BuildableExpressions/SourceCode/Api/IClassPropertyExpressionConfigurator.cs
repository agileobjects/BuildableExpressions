namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> for a
    /// <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassPropertyExpressionConfigurator :
        IConcreteTypePropertyExpressionConfigurator,
        IClassMemberExpressionConfigurator
    {
    }
}