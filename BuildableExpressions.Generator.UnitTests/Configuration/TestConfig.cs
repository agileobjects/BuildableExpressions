namespace AgileObjects.BuildableExpressions.Generator.UnitTests.Configuration
{
    using Generator.Configuration;

    internal class TestConfig : IConfig
    {
        public static readonly IConfig Default = new TestConfig("BuildableExpressions.Test");

        private const string _solutionPath =
            @"C:\Projects\BuildableExpressions\BuildableExpressions.sln";

        private const string _projectPath =
            @"C:\Projects\BuildableExpressions\BuildableExpressions.Test\BuildableExpressions.Test.csproj";

        public TestConfig(string rootNamespace)
        {
            if (!string.IsNullOrEmpty(rootNamespace))
            {
                RootNamespace = rootNamespace;
            }

            OutputDirectory = "bin\\Debug\\net461";
        }

        public string SolutionPath => _solutionPath;

        public string ProjectPath => _projectPath;

        public string TargetFramework => "net4*";

        public string RootNamespace { get; set; }

        public string OutputDirectory { get; set; }
    }
}
