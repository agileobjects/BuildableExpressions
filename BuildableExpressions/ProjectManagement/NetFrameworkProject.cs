#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
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

        public override bool AddIfMissing(IEnumerable<string> relativeFilePaths)
        {
            var newCompileElementFilePaths = relativeFilePaths
                .Select(filePath => new
                {
                    FilePath = filePath,
                    ExistingCompileElement = ProjectElements
                        .FirstOrDefault(e => IsCompileElementFor(e, filePath))
                })
                .Where(_ => _.ExistingCompileElement == null)
                .Select(_ => _.FilePath)
                .ToList();

            if (!newCompileElementFilePaths.Any())
            {
                return false;
            }

            return AddNewCompileItemGroup(newCompileElementFilePaths);
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

        private bool AddNewCompileItemGroup(IEnumerable<string> newCompileElementFilePaths)
        {
            var newCompileItemGroup = new object[]
            {
                new XText(NewLine + _indent),
                new XElement(
                    "ItemGroup",
                    newCompileElementFilePaths
                        .SelectMany(filePath => new object[]
                        {
                            new XText(NewLine + _indent + _indent),
                            new XElement("Compile", new XAttribute("Include", filePath))
                        })
                        .Concat(new[] { new XText(NewLine + _indent) }))
            };

            var finalExistingItemGroupElement = _projectXml
                .Descendants()
                .LastOrDefault(e => e.Name.LocalName == "ItemGroup");

            if (finalExistingItemGroupElement != null)
            {
                finalExistingItemGroupElement.AddAfterSelf(newCompileItemGroup);
                return true;
            }

            _projectXml.Root?.Add(newCompileItemGroup);
            return true;
        }
    }
}
#endif