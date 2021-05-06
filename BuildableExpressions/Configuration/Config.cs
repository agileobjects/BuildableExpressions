namespace AgileObjects.BuildableExpressions.Configuration
{
    using System.IO;

    internal class Config
    {
        public Config(string projectPath)
        {
            ProjectPath = projectPath;
            ContentRoot = Path.GetDirectoryName(projectPath);
        }

        public string ProjectPath { get; }

        public string ContentRoot { get; }

        public string RootNamespace { get; set; }
    }
}
