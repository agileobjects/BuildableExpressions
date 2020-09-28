namespace AgileObjects.BuildableExpressions.UnitTests.InputOutput
{
    using System.IO;
    using BuildableExpressions.InputOutput;
    using Configuration;
    using Moq;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static Moq.Times;

    public class OutputWriterTests
    {
        private const string _projectDirectory = @"C:\Data\VisualStudio\BuildableExpressions";
        private const string _rootNamespace = "AgileObjects.BuildableExpressions";

        [Fact]
        public void ShouldWriteToRootDirectory()
        {
            var fileManagerMock = new Mock<IFileManager>();
            var outputWriter = new OutputWriter(fileManagerMock.Object);

            var sourceCode = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace(_rootNamespace));

            var @class = sourceCode.AddClass();
            @class.AddMethod(Default(typeof(void)));

            var fileName = @class.Name + ".cs";

            outputWriter.Write(new[] { sourceCode }, new Config
            {
                ContentRoot = _projectDirectory,
                RootNamespace = _rootNamespace
            });

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

            var sourceCode = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace($"{_rootNamespace}.GeneratedCode"));

            var @class = sourceCode.AddClass();
            @class.AddMethod(Default(typeof(void)));

            var fileName = @class.Name + ".cs";

            outputWriter.Write(new[] { sourceCode }, new Config
            {
                ContentRoot = _projectDirectory,
                RootNamespace = _rootNamespace
            });

            var expectedOutputDirectory =
                Path.Combine(_projectDirectory, "GeneratedCode");

            fileManagerMock.Verify(fm => fm.EnsureDirectory(expectedOutputDirectory));

            fileManagerMock.Verify(fm => fm.Write(
                Path.Combine(expectedOutputDirectory, fileName),
                It.IsAny<string>()));
        }
    }
}
