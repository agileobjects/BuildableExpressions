namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    internal class NetCoreProject : ProjectBase
    {
        public NetCoreProject(XDocument projectXml)
            : base(projectXml)
        {
        }

        public override bool AddIfMissing(IEnumerable<string> relativeFilePaths) => false;
    }
}