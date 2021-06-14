namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using Configuration;

    internal interface IProjectFactory
    {
        IProject GetOutputProjectOrThrow(IConfig config);
    }
}