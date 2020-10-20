namespace AgileObjects.BuildableExpressions.UnitTests.InputOutput
{
    using System.IO;
    using System.Linq;
    using BuildableExpressions.InputOutput;
    using Configuration;
    using Moq;
    using SourceCode;
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

            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace(_rootNamespace);

                    sc.AddClass(cls =>
                    {
                        cls.AddMethod(Default(typeof(void)));
                    });
                });

            var fileName = sourceCode.TypeExpressions.First().Name + ".cs";

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

            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace($"{_rootNamespace}.GeneratedCode");

                sc.AddClass(cls =>
                {
                    cls.AddMethod(Default(typeof(void)));
                });
            });

            var fileName = sourceCode.TypeExpressions.First().Name + ".cs";

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
