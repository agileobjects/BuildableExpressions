namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Xml.Linq;

    internal class NetCoreProject : ProjectBase
    {
        public NetCoreProject(XDocument projectXml)
            : base(projectXml)
        {
        }

        public override bool Add(string[] relativeFilePaths) => false;
    }
}