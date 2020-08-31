#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Linq;
    using System.Xml.Linq;
    using static System.Environment;

    internal class NetFrameworkProject : ProjectBase
    {
        private const string _indent = "  ";

        private readonly XDocument _projectXml;

        public NetFrameworkProject(XDocument projectXml)
            : base(projectXml)
        {
            _projectXml = projectXml;
        }

        public override bool Add(string[] relativeFilePaths)
        {
            var filesAdded = false;

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

                filesAdded = true;
            }

            return filesAdded;
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
    }
}
#endif