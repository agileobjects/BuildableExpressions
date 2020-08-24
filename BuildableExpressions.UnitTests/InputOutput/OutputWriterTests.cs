namespace AgileObjects.BuildableExpressions.UnitTests.InputOutput
{
    using System;
    using System.IO;
    using System.Linq;
    using BuildableExpressions.InputOutput;
    using Configuration;
    using Moq;
    using ProjectManagement;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static SourceCodeFactory;

    public class OutputWriterTests
    {
        private const string _projectDirectory = @"C:\Data\VisualStudio\BuildableExpressions";
        private const string _rootNamespace = "AgileObjects.BuildableExpressions";

        [Fact]
        public void ShouldWriteToRootDirectory()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var outputWriter = new OutputWriter(
                fileManagerMock.Object,
                Mock.Of<IProjectManager>());

            var doNothing = SourceCode(
                Lambda<Action>(Default(typeof(void))),
                cfg => cfg.WithNamespace(_rootNamespace));

            var config = new Config
            {
                ProjectPath = Path.Combine(_projectDirectory, "BuildableExpressions.csproj"),
                RootNamespace = _rootNamespace,
                OutputDirectory = _projectDirectory
            };

            outputWriter.Write(new[] { doNothing }, config);

            fileManagerMock.Verify(fm => fm.EnsureDirectory(_projectDirectory));

            fileManagerMock.Verify(fm => fm.Write(
                @$"{_projectDirectory}\{doNothing.Classes.First().Name}.cs",
                It.IsAny<string>()));
        }

        [Fact]
        public void ShouldWriteToNamespaceDirectories()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var outputWriter = new OutputWriter(
                fileManagerMock.Object,
                Mock.Of<IProjectManager>());

            const string PROJECT_DIRECTORY = @"C:\Data\VisualStudio\BuildableExpressions";
            const string ROOT_NAMESPACE = "AgileObjects.BuildableExpressions";

            var doNothing = SourceCode(
                Lambda<Action>(Default(typeof(void))),
                cfg => cfg.WithNamespace($"{ROOT_NAMESPACE}.GeneratedCode"));

            var config = new Config
            {
                ProjectPath = Path.Combine(PROJECT_DIRECTORY, "BuildableExpressions.csproj"),
                RootNamespace = ROOT_NAMESPACE,
                OutputDirectory = PROJECT_DIRECTORY
            };

            outputWriter.Write(new[] { doNothing }, config);

            var expectedOutputDirectory =
                Path.Combine(PROJECT_DIRECTORY, "GeneratedCode");

            fileManagerMock.Verify(fm => fm.EnsureDirectory(expectedOutputDirectory));

            fileManagerMock.Verify(fm => fm.Write(
                @$"{expectedOutputDirectory}\{doNothing.Classes.First().Name}.cs",
                It.IsAny<string>()));
        }
    }
}
