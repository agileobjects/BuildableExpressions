namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal interface IRootNamespaceAccessor
    {
        string RootNamespace { get; }
    }

    internal interface IProjectManager : IRootNamespaceAccessor
    {
        void Load(string projectPath);

        void Add(string filePath);

        void Save();
    }
}
