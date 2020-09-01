namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;

    internal interface IProjectManager
    {
        void Init(string projectPath);

        void AddIfMissing(IEnumerable<string> relativeFilePaths);
    }
}