namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System.Collections.Generic;
    using Configuration;

    internal class SdkProject : IProject
    {
        public SdkProject(IConfig config, string rootNamespace)
        {
            FilePath = config.OutputProjectPath;
            RootNamespace = rootNamespace;
        }

        public string FilePath { get; }

        public string RootNamespace { get; }

        public void Add(IEnumerable<string> relativeFilePaths)
        {
        }
    }
}