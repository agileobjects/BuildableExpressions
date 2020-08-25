#if NET_STANDARD
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal class NullProjectManager : IProjectManager
    {
        public string RootNamespace => null;

        public void Load(string projectPath)
        {
        }

        public void Add(string filePath)
        {
        }

        public void Save()
        {
        }
    }
}
#endif