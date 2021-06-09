namespace AgileObjects.BuildableExpressions.Generator.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AgileObjects.BuildableExpressions.SourceCode;
    using Configuration;
    using Logging;
    using static System.StringComparison;

    internal class OutputWriter
    {
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        public OutputWriter(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;
        }

        public ICollection<string> Write(
            IConfig config,
            params SourceCodeExpression[] sourceCodeExpressions)
        {
            return Write(config, sourceCodeExpressions.AsEnumerable());
        }

        public ICollection<string> Write(
            IConfig config,
            IEnumerable<SourceCodeExpression> sourceCodeExpressions)
        {
            var rootNamespace = config.RootNamespace;
            var newFilePaths = new List<string>();

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var @namespace = sourceCodeExpression.Namespace;

                var outputDirectory = config.GetContentRoot();
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

                Write(filePath, sourceCode);
                newFilePaths.Add(Path.Combine(relativeFilePath, fileName));
            }

            return newFilePaths;
        }

        public void Write(string filePath, string content)
        {
            _fileManager.Write(filePath, content);
            _logger.Info($"output file '{Path.GetFileName(filePath)}' written");
        }
    }
}
