namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;
    using InputOutput;
    using static System.StringComparison;

    internal class ProjectManager : IProjectManager
    {
        private readonly IFileManager _fileManager;
        private string _projectFilePath;
        private ProjectBase _project;

        public ProjectManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public string RootNamespace => _project?.RootNamespace;

        public void Init(string projectFilePath)
        {
            var projectContent = _fileManager.Read(projectFilePath);

            var projectStartIndex = projectContent.IndexOf("<Project", Ordinal);

            if (projectStartIndex == -1)
            {
                throw new XmlException(
                    $"Unable to find <Project /> element in file '{projectFilePath}'");
            }

            var projectXml = XDocument.Parse(projectContent, LoadOptions.PreserveWhitespace);
            _projectFilePath = projectFilePath;

#if NETFRAMEWORK
            _project = projectContent.Contains("<Project Sdk=\"")
                ? (ProjectBase)new NetCoreProject(projectXml)
                : new NetFrameworkProject(projectXml);
#else
            _project = new NetCoreProject(projectXml);
#endif
        }

        public void AddIfMissing(IEnumerable<string> relativeFilePaths)
        {
            if (_project.AddIfMissing(relativeFilePaths))
            {
                _fileManager.Write(_projectFilePath, _project.GetContent());
            }
        }
    }
}