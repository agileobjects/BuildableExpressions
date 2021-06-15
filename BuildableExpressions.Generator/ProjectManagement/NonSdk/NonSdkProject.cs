#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement.NonSdk
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Configuration;

    internal class NetFrameworkProject : IProject
    {
        private readonly string _solutionName;

        public NetFrameworkProject(IConfig config, string rootNamespace)
        {
            _solutionName = config.GetSolutionName();
            FilePath = config.OutputProjectPath;
            RootNamespace = rootNamespace;
        }

        public string FilePath { get; }

        public string RootNamespace { get; }

        public void Add(IEnumerable<string> relativeFilePaths)
        {
            var contentRoot = Path.GetDirectoryName(FilePath);

            var devTools = DevToolsFactory
                .GetDevToolsOrNullFor(_solutionName);

            var project = devTools
                .Solution
                .EnumerateProjects()
                .First(p => p.FullName == FilePath);

            foreach (var filePath in relativeFilePaths)
            {
                var absoluteFilePath = Path.Combine(contentRoot!, filePath);
                project.ProjectItems.AddFromFile(absoluteFilePath);
            }
        }
    }
}
#endif