namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    internal interface IConfig
    {
        string SolutionPath { get; }

        string InputProjectPath { get; }

        string OutputProjectPath { get; }

        string TargetFramework { get; }

        string RootNamespace { get; set; }

        string InputDirectory { get; set; }
    }
}