namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    using System.IO;

    internal class Config
    {
        public Config(string projectPath, string rootNamespace)
        {
            ProjectPath = projectPath;
            ContentRoot = Path.GetDirectoryName(projectPath);

            if (!string.IsNullOrEmpty(rootNamespace))
            {
                RootNamespace = rootNamespace;
            }
        }

        public string ProjectPath { get; }

        public string ContentRoot { get; }

        public string RootNamespace { get; set; }
    }
}
