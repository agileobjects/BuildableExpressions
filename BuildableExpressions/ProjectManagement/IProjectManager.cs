namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal interface IProjectManager
    {
        void Load(string projectPath);

        void Add(string filePath);

        void Save();
    }
}
