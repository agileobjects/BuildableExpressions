#if NET_STANDARD
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    internal class NullProjectManager : IProjectManager
    {
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