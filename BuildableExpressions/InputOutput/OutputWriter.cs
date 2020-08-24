﻿namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Configuration;
    using SourceCode;
    using static System.StringComparison;

    internal class OutputWriter
    {
        private readonly IFileManager _fileManager;

        public OutputWriter(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Write(
            IEnumerable<SourceCodeExpression> sourceCodeExpressions,
            Config config)
        {
            var rootNamespace = config.RootNamespace;

            _fileManager.EnsureDirectory(config.OutputRoot);

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var @namespace = sourceCodeExpression.Namespace;

                var outputDirectory = config.OutputRoot;

                if (@namespace != rootNamespace &&
                    @namespace.StartsWith(rootNamespace, OrdinalIgnoreCase))
                {
                    @namespace = @namespace.Substring(rootNamespace.Length + 1);

                    outputDirectory = Path.Combine(
                        new[] { config.OutputRoot }.Concat(@namespace.Split('.')).ToArray());

                    _fileManager.EnsureDirectory(outputDirectory);
                }

                var fileName = sourceCodeExpression.Classes.First().Name + ".cs";
                var filePath = Path.Combine(outputDirectory, fileName);

                var sourceCode = sourceCodeExpression.ToSourceCode();

                _fileManager.Write(filePath, sourceCode);
            }
        }
    }
}
