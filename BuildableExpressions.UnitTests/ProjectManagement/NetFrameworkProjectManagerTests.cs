﻿namespace AgileObjects.BuildableExpressions.UnitTests.ProjectManagement
{
    using System.Linq;
    using System.Xml.Linq;
    using BuildableExpressions.InputOutput;
    using BuildableExpressions.ProjectManagement;
    using Common;
    using Moq;
    using Xunit;

    public class NetFrameworkProjectManagerTests
    {
        private const string _projectFile =
            @"C:\Projects\BuildableExpressions\BuildableExpressions.Test\BuildableExpressions.Test.csproj";

        private const string _newFilePath = "GeneratedCode/Blah/NewClass.cs";

        [Fact]
        public void ShouldFindProjectXmlRootNamespace()
        {
            var fileManagerMock = new Mock<IFileManager>();

            #region Project File Content

            fileManagerMock
                .Setup(fm => fm.Read(It.IsAny<string>()))
                .Returns(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <OutputType>Exe</OutputType>
    <RootNamespace>AgileObjects.BuildableExpressions.Console</RootNamespace>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
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
    <None Include=""App.config"" />
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
".Trim());

            #endregion

            var projectManager = new NetFrameworkProjectManager(fileManagerMock.Object);
            projectManager.Load(_projectFile);

            projectManager.RootNamespace.ShouldBe("AgileObjects.BuildableExpressions.Console");
        }

        [Fact]
        public void ShouldAddASourceCodeFileAndSave()
        {
            var fileManagerMock = new Mock<IFileManager>();

            #region Project File Content

            fileManagerMock
                .Setup(fm => fm.Read(It.IsAny<string>()))
                .Returns(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <OutputType>Exe</OutputType>
    <RootNamespace>AgileObjects.BuildableExpressions.Console</RootNamespace>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
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
    <None Include=""App.config"" />
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
".Trim());

            #endregion

            const string NEW_FILE_PATH = "GeneratedCode/Blah/NewClass.cs";
            var writtenProjectXml = default(string);

            fileManagerMock
                .Setup(fm => fm.Write(_projectFile, It.IsAny<string>()))
                .Callback((string filePath, string content) => writtenProjectXml = content);

            var projectManager = new NetFrameworkProjectManager(fileManagerMock.Object);

            projectManager.Load(_projectFile);
            projectManager.Add(NEW_FILE_PATH);
            projectManager.Save();

            writtenProjectXml.ShouldNotBeNull();

            XDocument
                .Parse(writtenProjectXml)
                .Root.ShouldNotBeNull()
                .Elements()
                .Where(e => e.Name.LocalName == "ItemGroup")
                .SelectMany(ig => ig.Elements().Where(e => e.Name.LocalName == "Compile"))
                .Single(ce => ce.Attribute("Include")?.Value == NEW_FILE_PATH)
                .ShouldNotBeNull();
        }

        [Fact]
        public void ShouldIgnoreSourceCodeFilesFromIncludeAttributes()
        {
            var fileManagerMock = new Mock<IFileManager>();

            #region Project File Content

            fileManagerMock
                .Setup(fm => fm.Read(It.IsAny<string>()))
                .Returns(@$"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <OutputType>Exe</OutputType>
    <RootNamespace>AgileObjects.BuildableExpressions.Console</RootNamespace>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""AgileObjects.NetStandardPolyfills, Version=1.4.1.0, Culture=neutral, PublicKeyToken=06131ac1c008ad4e, processorArchitecture=MSIL"">
      <HintPath>..\packages\AgileObjects.NetStandardPolyfills.1.4.1\lib\net40\AgileObjects.NetStandardPolyfills.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Program.cs"" />
    <Compile Include=""{_newFilePath}"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""App.config"" />
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
".Trim());

            #endregion

            var writtenProjectXml = default(string);

            fileManagerMock
                .Setup(fm => fm.Write(_projectFile, It.IsAny<string>()))
                .Callback((string filePath, string content) => writtenProjectXml = content);

            var projectManager = new NetFrameworkProjectManager(fileManagerMock.Object);

            projectManager.Load(_projectFile);
            projectManager.Add(_newFilePath);
            projectManager.Save();

            writtenProjectXml.ShouldBeNull();
        }

        [Fact]
        public void ShouldIgnoreSourceCodeFilesFromIncludeElements()
        {
            var fileManagerMock = new Mock<IFileManager>();

            #region Project File Content

            fileManagerMock
                .Setup(fm => fm.Read(It.IsAny<string>()))
                .Returns(@$"
<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <OutputType>Exe</OutputType>
    <RootNamespace>AgileObjects.BuildableExpressions.Console</RootNamespace>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""AgileObjects.NetStandardPolyfills, Version=1.4.1.0, Culture=neutral, PublicKeyToken=06131ac1c008ad4e, processorArchitecture=MSIL"">
      <HintPath>..\packages\AgileObjects.NetStandardPolyfills.1.4.1\lib\net40\AgileObjects.NetStandardPolyfills.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Program.cs"" />
    <Compile>
      <Include>{_newFilePath}</Include>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <None Include=""App.config"" />
    <None Include=""packages.config"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
".Trim());

            #endregion

            var writtenProjectXml = default(string);

            fileManagerMock
                .Setup(fm => fm.Write(_projectFile, It.IsAny<string>()))
                .Callback((string filePath, string content) => writtenProjectXml = content);

            var projectManager = new NetFrameworkProjectManager(fileManagerMock.Object);
            
            projectManager.Load(_projectFile);
            projectManager.Add(_newFilePath);
            projectManager.Save();

            writtenProjectXml.ShouldBeNull();
        }
    }
}
