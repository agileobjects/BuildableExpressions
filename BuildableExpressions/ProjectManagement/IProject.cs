namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;

    internal interface IProject
    {
        void Add(IEnumerable<string> relativeFilePaths);
    }
}