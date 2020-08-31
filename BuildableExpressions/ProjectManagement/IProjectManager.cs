namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal interface IProjectManager
    {
        void Init(string projectPath);

        void Add(params string[] relativeFilePaths);

        void Save();
    }
}