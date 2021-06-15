namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System.Collections.Generic;

    internal interface IProject
    {
        string FilePath { get; }

        string RootNamespace { get; }

        void Add(IEnumerable<string> relativeFilePaths);
    }
}