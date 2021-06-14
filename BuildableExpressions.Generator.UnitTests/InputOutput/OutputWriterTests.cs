﻿namespace AgileObjects.BuildableExpressions.Generator.UnitTests.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BuildableExpressions.Generator.InputOutput;
    using BuildableExpressions.SourceCode;
    using BuildableExpressions.UnitTests.Common;
    using Configuration;
    using Generator.Configuration;
    using Logging;
    using Moq;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static Moq.Times;

    public class OutputWriterTests
    {
        private static readonly string _projectDirectory = TestConfig.Default.GetOutputRoot();
        private const string _rootNamespace = "AgileObjects.BuildableExpressions";

        [Fact]
        public void ShouldWriteToRootDirectory()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var outputWriter = new OutputWriter(Mock.Of<ILogger>(), fileManagerMock.Object);

            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace(_rootNamespace);

                    sc.AddClass(cls =>
                    {
                        cls.AddMethod(Empty());
                    });
                });

            var fileName = sourceCode.TypeExpressions.First().Name + ".cs";

            outputWriter.Write(new TestConfig(_rootNamespace), sourceCode);

            fileManagerMock.Verify(fm => fm.EnsureDirectory(_projectDirectory), Never);

            fileManagerMock.Verify(fm => fm.Write(
                Path.Combine(_projectDirectory, fileName),
                It.IsAny<string>()));
        }

        [Fact]
        public void ShouldWriteToNamespaceDirectories()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var outputWriter = new OutputWriter(Mock.Of<ILogger>(), fileManagerMock.Object);

            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace($"{_rootNamespace}.GeneratedCode");

                sc.AddClass(cls =>
                {
                    cls.AddMethod(Empty());
                });
            });

            var fileName = sourceCode.TypeExpressions.First().Name + ".cs";

            outputWriter.Write(
                new TestConfig(_rootNamespace),
                new List<SourceCodeExpression> { sourceCode });

            var expectedOutputDirectory =
                Path.Combine(_projectDirectory, "GeneratedCode");

            fileManagerMock.Verify(fm => fm.EnsureDirectory(expectedOutputDirectory));

            fileManagerMock.Verify(fm => fm.Write(
                Path.Combine(expectedOutputDirectory, fileName),
                It.IsAny<string>()));
        }
    }
}
