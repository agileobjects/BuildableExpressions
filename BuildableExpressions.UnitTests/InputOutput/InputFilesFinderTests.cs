namespace AgileObjects.BuildableExpressions.UnitTests.InputOutput
{
    using System;
    using System.IO;
    using BuildableExpressions.InputOutput;
    using Common;
    using Configuration;
    using Logging;
    using Moq;
    using Xunit;
    using static BuildConstants;
    using static Moq.Times;

    public class InputFilesFinderTests
    {
        private const string _contentRoot = @"C:\Data\VisualStudio\BuildableExpressions";
        private const string _projectFilePath = _contentRoot + @"\MyProject.csproj";
        private const string _rootNamespace = "AgileObjects.BuildableExpressions";

        [Fact]
        public void ShouldAddTheDefaultInputFile()
        {
            var fileManagerMock = new Mock<IFileManager>();

            fileManagerMock
                .Setup(fm => fm.FindFiles(_contentRoot, "*.cs"))
                .Returns(Array.Empty<string>());

            var finder = new InputFilesFinder(fileManagerMock.Object, Mock.Of<ILogger>())
            {
                Config = new Config(_projectFilePath)
                {
                    RootNamespace = _rootNamespace
                }
            };

            var inputFiles = finder.GetInputFiles();

            var inputFile = inputFiles.ShouldHaveSingleItem();
            inputFile.FilePath.ShouldBe(Path.Combine(_contentRoot, DefaultInputFileName));
            inputFile.Contents.ShouldNotContain(DefaultInputFileNamespace);
            inputFile.Contents.ShouldContain(_rootNamespace);

            var expectedOutputFilePath = Path.Combine(_contentRoot, DefaultInputFileName);
            fileManagerMock.Verify(fm => fm.Write(expectedOutputFilePath, It.IsAny<string>()), Once);
        }

        [Fact]
        public void ShouldFindMultipleExistingInputFiles()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var filePath1 = @$"{_contentRoot}\File1.cs";
            var filePath2 = @$"{_contentRoot}\Types\File2.cs";

            fileManagerMock
                .Setup(fm => fm.FindFiles(_contentRoot, "*.cs"))
                .Returns(new[] { filePath1, filePath2 });

            fileManagerMock
                .Setup(fm => fm.Read(filePath1))
                .Returns(@$"
namespace {DefaultInputFileNamespace}
{{
    public class Class1 : BuildableExpressions.ISourceCodeExpressionBuilder
    {{
    }}
}}");
            fileManagerMock
                .Setup(fm => fm.Read(filePath2))
                .Returns(@$"
namespace {DefaultInputFileNamespace}
{{
    public class Class2 : BuildableExpressions.ISourceCodeExpressionBuilder
    {{
    }}
}}");
            var finder = new InputFilesFinder(fileManagerMock.Object, Mock.Of<ILogger>())
            {
                Config = new Config(_projectFilePath)
                {
                    RootNamespace = _rootNamespace
                }
            };

            var inputFiles = finder.GetInputFiles();

            inputFiles.Count.ShouldBe(2);

            fileManagerMock.Verify(fm => fm.Write(It.IsAny<string>(), It.IsAny<string>()), Never);
        }

        [Fact]
        public void ShouldFilterBinAndObjFolderFiles()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var filePath1 = @$"{_contentRoot}\bin\Debug\File1.cs";
            var filePath2 = @$"{_contentRoot}\obj\File2.cs";
            var filePath3 = @$"{_contentRoot}\File3.cs";

            fileManagerMock
                .Setup(fm => fm.FindFiles(_contentRoot, "*.cs"))
                .Returns(new[] { filePath1, filePath2, filePath3 });

            fileManagerMock
                .Setup(fm => fm.Read(filePath3))
                .Returns(@$"
namespace {DefaultInputFileNamespace}
{{
    public class Class3 : BuildableExpressions.ISourceCodeExpressionBuilder
    {{
    }}
}}");
            var finder = new InputFilesFinder(fileManagerMock.Object, Mock.Of<ILogger>())
            {
                Config = new Config(_projectFilePath)
                {
                    RootNamespace = _rootNamespace
                }
            };

            var inputFiles = finder.GetInputFiles();

            inputFiles.ShouldHaveSingleItem().FilePath.ShouldBe(filePath3);

        }

        [Fact]
        public void ShouldFilterNonInputFiles()
        {
            var fileManagerMock = new Mock<IFileManager>();

            var filePath1 = @$"{_contentRoot}\File1.cs";
            var filePath2 = @$"{_contentRoot}\File2.cs";

            fileManagerMock
                .Setup(fm => fm.FindFiles(_contentRoot, "*.cs"))
                .Returns(new[] { filePath1, filePath2 });

            fileManagerMock
                .Setup(fm => fm.Read(filePath1))
                .Returns(@$"
namespace {DefaultInputFileNamespace}
{{
    public class Class1 : BuildableExpressions.ISourceCodeExpressionBuilder
    {{
    }}
}}");
            fileManagerMock
                .Setup(fm => fm.Read(filePath2))
                .Returns(@"
namespace MyOtherClassNamespace
{
    public class Class2
    {
    }
}");
            var finder = new InputFilesFinder(fileManagerMock.Object, Mock.Of<ILogger>())
            {
                Config = new Config(_projectFilePath)
                {
                    RootNamespace = _rootNamespace
                }
            };

            var inputFiles = finder.GetInputFiles();

            inputFiles.ShouldHaveSingleItem().FilePath.ShouldBe(filePath1);

        }
    }
}
