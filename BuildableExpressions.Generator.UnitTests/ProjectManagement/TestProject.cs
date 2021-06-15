namespace AgileObjects.BuildableExpressions.Generator.UnitTests.ProjectManagement
{
    using System.Collections.Generic;
    using System.IO;
    using Generator.ProjectManagement;
    using static TestConstants;

    internal class TestProject : IProject
    {
        public static readonly IProject Default = new TestProject();

        public string FilePath => TestProjectPath;

        public string RootNamespace => Path.GetFileNameWithoutExtension(TestProjectPath);

        public void Add(IEnumerable<string> relativeFilePaths)
        {
        }
    }
}
