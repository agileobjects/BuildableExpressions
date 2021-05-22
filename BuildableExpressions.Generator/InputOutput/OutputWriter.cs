namespace AgileObjects.BuildableExpressions.Generator.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AgileObjects.BuildableExpressions.SourceCode;
    using Configuration;
    using static System.StringComparison;

    internal class OutputWriter
    {
        private readonly IFileManager _fileManager;
        private readonly Config _config;

        public OutputWriter(IFileManager fileManager, Config config)
        {
            _fileManager = fileManager;
            _config = config;
        }

        public List<string> Write(params SourceCodeExpression[] sourceCodeExpressions)
            => Write(sourceCodeExpressions.AsEnumerable());

        public List<string> Write(IEnumerable<SourceCodeExpression> sourceCodeExpressions)
        {
            var rootNamespace = _config.RootNamespace;
            var newFilePaths = new List<string>();

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var @namespace = sourceCodeExpression.Namespace;

                var outputDirectory = _config.ContentRoot;
                string relativeFilePath;

                if (@namespace != rootNamespace &&
                    @namespace.StartsWith(rootNamespace, OrdinalIgnoreCase))
                {
                    @namespace = @namespace.Substring(rootNamespace.Length + 1);

                    relativeFilePath = Path.Combine(@namespace.Split('.'));
                    outputDirectory = Path.Combine(outputDirectory, relativeFilePath);

                    _fileManager.EnsureDirectory(outputDirectory);
                }
                else
                {
                    relativeFilePath = string.Empty;
                }

                var fileName = sourceCodeExpression.TypeExpressions.First().Name + ".cs";
                var filePath = Path.Combine(outputDirectory, fileName);

                var sourceCode = sourceCodeExpression.ToCSharpString();

                _fileManager.Write(filePath, sourceCode);
                newFilePaths.Add(Path.Combine(relativeFilePath, fileName));
            }

            return newFilePaths;
        }
    }
}
