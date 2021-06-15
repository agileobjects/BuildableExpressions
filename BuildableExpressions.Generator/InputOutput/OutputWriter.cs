namespace AgileObjects.BuildableExpressions.Generator.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AgileObjects.BuildableExpressions.SourceCode;
    using Logging;
    using ProjectManagement;
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
            IProject project,
            IEnumerable<SourceCodeExpression> sourceCodeExpressions)
        {
            var outputRoot = Path.GetDirectoryName(project.FilePath);
            var rootNamespace = project.RootNamespace;
            var newFilePaths = new List<string>();

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var outputDirectory = outputRoot;
                var @namespace = sourceCodeExpression.Namespace;

                string relativeFilePath;

                if (@namespace != rootNamespace &&
                    @namespace.StartsWith(rootNamespace, OrdinalIgnoreCase))
                {
                    var relativeNamespace = @namespace.Substring(rootNamespace.Length + 1);

                    relativeFilePath = Path.Combine(relativeNamespace.Split('.'));
                    outputDirectory = Path.Combine(outputDirectory!, relativeFilePath);

                    _fileManager.EnsureDirectory(outputDirectory);
                }
                else
                {
                    relativeFilePath = string.Empty;
                }

                var csFileName = sourceCodeExpression.TypeExpressions.First().Name + ".cs";
                var csFilePath = Path.Combine(outputDirectory!, csFileName);

                var sourceCode = sourceCodeExpression.ToCSharpString();

                Write(csFilePath, sourceCode);
                newFilePaths.Add(Path.Combine(relativeFilePath, csFileName));
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
