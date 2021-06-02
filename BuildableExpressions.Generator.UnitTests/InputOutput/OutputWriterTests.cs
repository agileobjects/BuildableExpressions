﻿namespace AgileObjects.BuildableExpressions.Generator.UnitTests.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BuildableExpressions.Generator.InputOutput;
    using BuildableExpressions.SourceCode;
    using BuildableExpressions.UnitTests.Common;
    using Configuration;
    using Moq;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static Moq.Times;

    public class OutputWriterTests
    {
        private const string _projectDirectory = @"C:\Data\VisualStudio\BuildableExpressions";
        private const string _solutionPath = _projectDirectory + @"\MySolution.sln";
        private const string _projectPath = _projectDirectory + @"\MyProject.csproj";
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
                        cls.AddMethod(Empty());
                    });
                });

            var fileName = sourceCode.TypeExpressions.First().Name + ".cs";

            outputWriter.Write(
                new Config(_solutionPath, _projectPath, _rootNamespace),
                sourceCode);

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
                    cls.AddMethod(Empty());
                });
            });

            var fileName = sourceCode.TypeExpressions.First().Name + ".cs";

            outputWriter.Write(
                new Config(_solutionPath, _projectPath, _rootNamespace),
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
