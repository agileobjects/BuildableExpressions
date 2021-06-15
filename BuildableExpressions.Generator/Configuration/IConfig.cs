namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    internal interface IConfig
    {
        string SolutionPath { get; }

        string InputProjectPath { get; }

        string OutputProjectPath { get; }

        string TargetFramework { get; }

        string InputDirectory { get; }
    }
}