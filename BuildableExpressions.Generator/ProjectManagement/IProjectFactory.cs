namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using Configuration;

    internal interface IProjectFactory
    {
        IProject GetProject(Config config);
    }
}