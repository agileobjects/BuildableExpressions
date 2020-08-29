#if NET_STANDARD
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal class NullProjectManager : IProjectManager
    {
        public string RootNamespace => null;

        public void Load(string projectFilePath)
        {
        }

        public void Add(params string[] relativeFilePaths)
        {
        }

        public void Save()
        {
        }
    }
}
#endif