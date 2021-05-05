#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Configuration;
    using EnvDTE;

    internal class NetFrameworkProject : IProject
    {
        private readonly Config _config;

        public NetFrameworkProject(Config config)
        {
            _config = config;
        }

        public void Add(IEnumerable<string> relativeFilePaths)
        {
            var dte = (DTE)Marshal.GetActiveObject("VisualStudio.DTE");

            var project = dte
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