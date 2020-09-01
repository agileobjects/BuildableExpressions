namespace AgileObjects.BuildableExpressions.UnitTests.InputOutput
{
    using System;
    using System.IO;
    using System.Linq;
    using BuildableExpressions.InputOutput;
    using Configuration;
    using Moq;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static Moq.Times;
    using static SourceCodeFactory;

    public class OutputWriterTests
    {
        private const string _projectDirectory = @"C:\Data\VisualStudio\BuildableExpressions";
        private const string _rootNamespace = "AgileObjects.BuildableExpressions";

        [Fact]
        public void ShouldWriteToRootDirectory()
        {
            var fileManagerMock = new Mock<IFileManager>();
            var outputWriter = new OutputWriter(fileManagerMock.Object);

            var doNothing = SourceCode(
                Lambda<Action>(Default(typeof(void))),
                cfg => cfg.WithNamespace(_rootNamespace));

            var config = new Config
            {
                ContentRoot = _projectDirectory,
                RootNamespace = _rootNamespace
            };

            var fileName = doNothing.Classes.First().Name + ".cs";

            outputWriter.Write(new[] { doNothing }, config);

            fileManagerMock.Verify(fm => fm.EnsureDirectory(_projectDirectory), Never);

            fileManagerMock.Verify(fm => fm.Write(
                Path.Combine(_projectDirectory, fileName),
                It.IsAny<string>()));
        }

        [Fact]
        public void ShouldWriteToNamespaceDirectories()
        {
            var fileManagerMock = new Mock<IFileManager>();
            var outputWriter = new OutputWriter(fileManagerMock.Object);

            var doNothing = SourceCode(
                Lambda<Action>(Default(typeof(void))),
                cfg => cfg.WithNamespace($"{_rootNamespace}.GeneratedCode"));

            var config = new Config
            {
                ContentRoot = _projectDirectory,
                RootNamespace = _rootNamespace
            };

            var fileName = doNothing.Classes.First().Name + ".cs";

            outputWriter.Write(new[] { doNothing }, config);

            var expectedOutputDirectory =
                Path.Combine(_projectDirectory, "GeneratedCode");

            fileManagerMock.Verify(fm => fm.EnsureDirectory(expectedOutputDirectory));

            fileManagerMock.Verify(fm => fm.Write(
                Path.Combine(expectedOutputDirectory, fileName),
                It.IsAny<string>()));
        }
    }
}
