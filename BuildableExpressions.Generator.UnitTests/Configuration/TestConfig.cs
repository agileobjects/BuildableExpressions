namespace AgileObjects.BuildableExpressions.Generator.UnitTests.Configuration
{
    using Generator.Configuration;
    using static TestConstants;

    internal class TestConfig : IConfig
    {
        public static readonly IConfig Default = new TestConfig();

        public TestConfig()
        {
            InputDirectory = "bin\\Debug\\net461";
            OutputProjectPath = TestProjectPath;
        }

        public string SolutionPath => TestSolutionPath;

        public string InputProjectPath => TestProjectPath;

        public string OutputProjectPath { get; set; }

        public string TargetFramework => "net4*";

        public string InputDirectory { get; set; }
    }
}
