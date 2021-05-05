namespace AgileObjects.BuildableExpressions.InputOutput
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

        public Config Config { get; set; }

        public List<string> Write(params SourceCodeExpression[] sourceCodeExpressions)
            => Write(sourceCodeExpressions.AsEnumerable());

        public List<string> Write(IEnumerable<SourceCodeExpression> sourceCodeExpressions)
        {
            var rootNamespace = Config.RootNamespace;
            var newFilePaths = new List<string>();

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var @namespace = sourceCodeExpression.Namespace;

                var outputDirectory = Config.ContentRoot;
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
