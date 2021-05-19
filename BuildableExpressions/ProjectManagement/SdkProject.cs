namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System.Collections.Generic;
    using Compilation;
    using Configuration;

    internal class SdkProject : IProject
    {
        public SdkProject(AssemblyResolver assemblyResolver, Config config)
        {
            assemblyResolver.Init(config);
        }

        public void Add(IEnumerable<string> relativeFilePaths)
        {
        }
    }
}