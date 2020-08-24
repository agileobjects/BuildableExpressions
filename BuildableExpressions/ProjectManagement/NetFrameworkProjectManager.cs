#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Linq;
    using Microsoft.Build.Evaluation;

    internal class NetFrameworkProjectManager : IProjectManager
    {
        private Project _project;

        public void Load(string projectPath)
            => _project = new Project(projectPath);

        public void Add(string filePath)
        {
            if (_project.Items.All(i => i.EvaluatedInclude != filePath))
            {
                _project.AddItem("Compile", filePath);
            }
        }

        public void Save() => _project.Save();
    }
}
#endif