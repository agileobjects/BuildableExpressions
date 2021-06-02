namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    using System.IO;

    internal class Config
    {
        public Config(
            string solutionPath,
            string projectPath, 
            string rootNamespace)
        {
            SolutionPath = solutionPath;
            SolutionName = Path.GetFileName(solutionPath);
            ProjectPath = projectPath;
            ContentRoot = Path.GetDirectoryName(projectPath);

            if (!string.IsNullOrEmpty(rootNamespace))
            {
                RootNamespace = rootNamespace;
            }
        }

        public string SolutionPath { get; }

        public string SolutionName { get; }

        public string ProjectPath { get; }

        public string ContentRoot { get; }

        public string RootNamespace { get; set; }
    }
}
