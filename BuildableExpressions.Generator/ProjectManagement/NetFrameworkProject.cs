#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Configuration;

    internal class NetFrameworkProject : IProject
    {
        private readonly Config _config;

        public NetFrameworkProject(Config config)
        {
            _config = config;
        }

        public void Add(IEnumerable<string> relativeFilePaths)
        {
            var devTools = DevToolsFactory
                .GetDevToolsOrNullFor(_config.SolutionName);

            var project = devTools
                .Solution
                .EnumerateProjects()
                .First(p => p.FullName == _config.ProjectPath);

            foreach (var filePath in relativeFilePaths)
            {
                var absoluteFilePath = Path.Combine(_config.ContentRoot, filePath);
                project.ProjectItems.AddFromFile(absoluteFilePath);
            }
        }
    }
}
#endif