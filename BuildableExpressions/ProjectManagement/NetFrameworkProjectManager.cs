#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Evaluation;

    internal class NetFrameworkProjectManager : IProjectManager
    {
        private Project _project;
        private bool _filesAdded;

        public string RootNamespace
            => _project.GetPropertyValue(nameof(RootNamespace));

        public void Load(string projectPath)
        {
            _project = ProjectCollection
                .GlobalProjectCollection
                .LoadProject(Path.GetFileName(projectPath));
        }

        public void Add(string filePath)
        {
            if (_project.Items.All(i => i.EvaluatedInclude != filePath))
            {
                _project.AddItem("Compile", filePath);
                _filesAdded = true;
            }
        }

        public void Save()
        {
            if (_filesAdded)
            {
                _project.Save();
            }
        }
    }
}
#endif