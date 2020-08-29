namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal interface IProjectManager : IRootNamespaceAccessor
    {
        void Load(string projectFilePath);

        void Add(params string[] relativeFilePaths);

        void Save();
    }
}
