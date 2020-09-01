namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    internal abstract class ProjectBase
    {
        private readonly XDocument _projectXml;

        protected ProjectBase(XDocument projectXml)
        {
            _projectXml = projectXml;
        }

        public string RootNamespace => ProjectElements
            .FirstOrDefault(e => e.Name.LocalName == "RootNamespace")?
            .Value;

        protected IEnumerable<XElement> ProjectElements
            => _projectXml.Descendants();

        public abstract bool AddIfMissing(IEnumerable<string> relativeFilePaths);

        public string GetContent()
        {
            return
                _projectXml.Declaration +
                _projectXml.ToString().Replace("<ItemGroup xmlns=\"\">", "<ItemGroup>");
        }
    }
}