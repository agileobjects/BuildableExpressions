namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System.Collections.Generic;

    internal interface IProject
    {
        void Add(IEnumerable<string> relativeFilePaths);
    }
}