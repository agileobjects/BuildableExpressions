#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Configuration;

    internal class NetFrameworkProject : IProject
    {
        private readonly IConfig _config;

        public NetFrameworkProject(IConfig config)
        {
            _config = config;
        }

        public void Add(IEnumerable<string> relativeFilePaths)
        {
            var solutionName = _config.GetSolutionName();
            var contentRoot = _config.GetContentRoot();

            var devTools = DevToolsFactory
                .GetDevToolsOrNullFor(solutionName);

            var project = devTools
                .Solution
                .EnumerateProjects()
                .First(p => p.FullName == _config.ProjectPath);

            foreach (var filePath in relativeFilePaths)
            {
                var absoluteFilePath = Path.Combine(contentRoot, filePath);
                project.ProjectItems.AddFromFile(absoluteFilePath);
            }
        }
    }
}
#endif