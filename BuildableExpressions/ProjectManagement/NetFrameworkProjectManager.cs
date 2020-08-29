#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using InputOutput;
    using static System.Environment;
    using static System.StringComparison;
    using static System.Xml.Linq.LoadOptions;

    internal class NetFrameworkProjectManager : IProjectManager
    {
        private const string _indent = "  ";

        private readonly IFileManager _fileManager;
        private string _projectFilePath;
        private XDocument _projectXml;
        private bool _filesAdded;

        public NetFrameworkProjectManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public string RootNamespace => ProjectElements
            .FirstOrDefault(e => e.Name.LocalName == "RootNamespace")?
            .Value;

        private IEnumerable<XElement> ProjectElements
            => _projectXml.Descendants();

        public void Load(string projectFilePath)
        {
            var projectContent = _fileManager.Read(projectFilePath);

            var projectStartIndex = projectContent.IndexOf("<Project", Ordinal);

            if (projectStartIndex == -1)
            {
                throw new XmlException(
                    $"Unable to find opening <Project /> element in file '{projectFilePath}'");
            }

            _projectXml = XDocument.Parse(projectContent, PreserveWhitespace);
            _projectFilePath = projectFilePath;
        }

        public void Add(params string[] relativeFilePaths)
        {
            foreach (var filePath in relativeFilePaths)
            {
                var existingCompileElement = ProjectElements
                    .FirstOrDefault(e => IsCompileElementFor(e, filePath));

                if (existingCompileElement != null)
                {
                    continue;
                }

                var newCompileItemGroup = new object[]
                {
                    new XText(NewLine + _indent),
                    new XElement(
                        "ItemGroup",
                        new XText(NewLine + _indent + _indent),
                        new XElement("Compile", new XAttribute("Include", filePath)),
                        new XText(NewLine + _indent))
                };

                var finalExistingItemGroupElement = _projectXml
                    .Descendants()
                    .LastOrDefault(e => e.Name.LocalName == "ItemGroup");

                if (finalExistingItemGroupElement != null)
                {
                    finalExistingItemGroupElement.AddAfterSelf(newCompileItemGroup);
                }
                else
                {
                    _projectXml.Root?.Add(newCompileItemGroup);
                }

                _filesAdded = true;
            }
        }

        private static bool IsCompileElementFor(XElement element, string filePath)
        {
            if (element.Name.LocalName != "Compile")
            {
                return false;
            }

            if (element.Attributes("Include").Any(a => a.Value == filePath))
            {
                return true;
            }

            return element
                .Descendants()
                .Any(e => e.Name.LocalName == "Include" && e.Value == filePath);
        }

        public void Save()
        {
            if (_filesAdded)
            {
                var content =
                    _projectXml.Declaration +
                    _projectXml.ToString().Replace("<ItemGroup xmlns=\"\">", "<ItemGroup>");

                _fileManager.Write(_projectFilePath, content);
            }
        }
    }
}
#endif