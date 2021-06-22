﻿// This generator creates a settings class based on the contents of any *.xmlsettings
// files in this project's output directory.

// This generator uses magic strings to create its output class, instead of the 
// BuildableExpressions configuration API.

// This is a .NET 4.7.2 class library in a non-SDK project.

// This sample is based on the .NET 5 Source Generators sample:
// https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/SettingsXmlGenerator.cs

namespace AgileObjects.BuildableExpressions.Generator.Samples.XmlSettingsGenerator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using AgileObjects.BuildableExpressions;
    using SourceCode;

    public class XmlToSettingsGenerator : ISourceCodeExpressionBuilder
    {
        public IEnumerable<SourceCodeExpression> Build(IExpressionBuildContext context)
        {
            var xmlSettingsFilePaths = Directory
                .EnumerateFiles(context.InputProjectOutputPath, "*.xmlsettings");

            foreach (var xmlSettingsFilePath in xmlSettingsFilePaths)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlSettingsFilePath);

                var className = xmlDoc.DocumentElement.GetAttribute("className");

                var classBuilder = new StringBuilder();

                classBuilder.Append(@$"
// Generated by XmlToSettingsGenerator in this project 

using System;

namespace {context.RootNamespace}
{{
    public class {className}
    {{");

                for (var i = 0; i < xmlDoc.DocumentElement.ChildNodes.Count; i++)
                {
                    var setting = (XmlElement)xmlDoc.DocumentElement.ChildNodes[i];
                    var settingType = setting.GetAttribute("type");

                    var settingValue = settingType switch
                    {
                        "string" => $"\"{setting.InnerText}\"",
                        "DateTime" => $"DateTime.Parse(\"{setting.InnerText}\")",
                        _ => setting.InnerText
                    };

                    classBuilder.Append(@$"
        public {settingType} {setting.Name} => {settingValue};
");
                }

                classBuilder.Append(@$"
    }}
}}");
                yield return BuildableExpression.SourceCode(classBuilder.ToString());
            }
        }
    }
}
