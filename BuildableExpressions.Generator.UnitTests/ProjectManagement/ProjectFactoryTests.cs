namespace AgileObjects.BuildableExpressions.Generator.UnitTests.ProjectManagement
{
    using System;
    using System.IO;
    using System.Text;
    using BuildableExpressions.Generator.InputOutput;
    using BuildableExpressions.Generator.ProjectManagement;
    using BuildableExpressions.UnitTests.Common;
    using Configuration;
    using Logging;
    using Moq;
    using Xunit;

    public class ProjectFactoryTests
    {
        private const string _solutionPath =
            @"C:\Projects\BuildableExpressions\BuildableExpressions.sln";

        private const string _projectPath =
            @"C:\Projects\BuildableExpressions\BuildableExpressions.Test\BuildableExpressions.Test.csproj";

        [Fact]
        public void ShouldErrorIfNonProjectFile()
        {
            var fileManager = CreateFileManager("Hi! I am a file");
            var factory = new ProjectFactory(Mock.Of<ILogger>(), fileManager);

            var projectEx = Should.Throw<NotSupportedException>(() =>
            {
                factory.GetProjectOrThrow(
                    new Config(_solutionPath, _projectPath, rootNamespace: null));
            });

            projectEx.Message.ShouldContain("Unable to find <Project />");
            projectEx.Message.ShouldContain(_projectPath);
        }

        [Fact]
        public void ShouldErrorIfNonProjectXmlFile()
        {
            #region File Contents
            const string fileContents = @"
<xml />
<Something>
   <LaLaLa />
   <DooDeeDoo>YEAH!</DooDeeDoo>
</Something>";
            #endregion

            var fileManager = CreateFileManager(fileContents);
            var factory = new ProjectFactory(Mock.Of<ILogger>(), fileManager);

            var projectEx = Should.Throw<NotSupportedException>(() =>
            {
                factory.GetProjectOrThrow(
                    new Config(_solutionPath, _projectPath, rootNamespace: null));
            });

            projectEx.Message.ShouldContain("Unable to find <Project />");
            projectEx.Message.ShouldContain(_projectPath);
        }

        [Fact]
        public void ShouldHandleAnSdkProject()
        {
            #region File Contents
            const string fileContents = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net461;netstandard1.5</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""AgileObjects.ReadableExpressions"" Version=""3.0.0-preview1"" />
  </ItemGroup>
</Project>";
            #endregion

            var fileManager = CreateFileManager(fileContents);
            var factory = new ProjectFactory(Mock.Of<ILogger>(), fileManager);

            var project = factory.GetProjectOrThrow(
                new Config(_solutionPath, _projectPath, rootNamespace: null));

            project.ShouldBeOfType<SdkProject>();
        }

        [Fact]
        public void ShouldPopulateAnSdkProjectRootNamespace()
        {
            #region File Contents
            const string fileContents = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net461</TargetFramework>
    <RootNamespace>AgileObjects.BuildableExpressions.Generator.UnitTests</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""AgileObjects.ReadableExpressions"" Version=""3.0.0-preview1"" />
  </ItemGroup>
</Project>";
            #endregion

            var fileManager = CreateFileManager(fileContents);
            var config = new Config(_solutionPath, _projectPath, rootNamespace: null);
            var factory = new ProjectFactory(Mock.Of<ILogger>(), fileManager);

            factory.GetProjectOrThrow(config);

            config.RootNamespace.ShouldBe("AgileObjects.BuildableExpressions.Generator.UnitTests");
        }

#if NETFRAMEWORK
        [Fact]
        public void ShouldHandleANetFrameworkProject()
        {
            #region File Contents
            const string fileContents = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""AgileObjects.NetStandardPolyfills, Version=1.4.1.0, Culture=neutral, PublicKeyToken=06131ac1c008ad4e, processorArchitecture=MSIL"">
      <HintPath>..\packages\AgileObjects.NetStandardPolyfills.1.4.1\lib\net40\AgileObjects.NetStandardPolyfills.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Program.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";
            #endregion

            var fileManager = CreateFileManager(fileContents);
            var factory = new ProjectFactory(Mock.Of<ILogger>(), fileManager);

            var project = factory.GetProjectOrThrow(
                new Config(_solutionPath, _projectPath, rootNamespace: string.Empty));

            project.ShouldBeOfType<NetFrameworkProject>();
        }

        [Fact]
        public void ShouldRetrieveANetFrameworkProjectRootNamespace()
        {
            #region File Contents
            const string fileContents = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <RootNamespace>AgileObjects.BuildableExpressions.Generator.UnitTests</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""AgileObjects.NetStandardPolyfills, Version=1.4.1.0, Culture=neutral, PublicKeyToken=06131ac1c008ad4e, processorArchitecture=MSIL"">
      <HintPath>..\packages\AgileObjects.NetStandardPolyfills.1.4.1\lib\net40\AgileObjects.NetStandardPolyfills.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Program.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";
            #endregion

            var fileManager = CreateFileManager(fileContents);
            var config = new Config(_solutionPath, _projectPath, rootNamespace: string.Empty);
            var factory = new ProjectFactory(Mock.Of<ILogger>(), fileManager);

            factory.GetProjectOrThrow(config);

            config.RootNamespace.ShouldBe("AgileObjects.BuildableExpressions.Generator.UnitTests");
        }
#endif
        #region Helper Members

        private static IFileManager CreateFileManager(string fileContents)
        {
            var stubFileStream = new MemoryStream(
                Encoding.UTF8.GetBytes(fileContents.TrimStart()));

            var fileManagerMock = new Mock<IFileManager>();
            fileManagerMock
                .Setup(fm => fm.OpenRead(_projectPath))
                .Returns(stubFileStream);

            return fileManagerMock.Object;
        }

        #endregion
    }
}
