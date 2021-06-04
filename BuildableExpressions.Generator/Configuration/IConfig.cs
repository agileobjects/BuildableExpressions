namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    internal interface IConfig
    {
        public string SolutionPath { get; }

        public string ProjectPath { get; }

        public string RootNamespace { get; set; }

        public string OutputDirectory { get; set; }
    }
}