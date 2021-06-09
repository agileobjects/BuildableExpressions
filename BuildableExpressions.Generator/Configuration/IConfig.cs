namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    internal interface IConfig
    {
        string SolutionPath { get; }

        string ProjectPath { get; }

        string RootNamespace { get; set; }

        string OutputDirectory { get; set; }
    }
}