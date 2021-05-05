namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using Configuration;

    internal interface IProjectFactory
    {
        IProject GetProject(Config config);
    }
}