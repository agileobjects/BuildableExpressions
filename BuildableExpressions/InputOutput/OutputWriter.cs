namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SourceCode;

    internal class OutputWriter
    {
        private readonly IFileManager _fileManager;

        public OutputWriter(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Write(
            IEnumerable<SourceCodeExpression> sourceCodeExpressions,
            string outputRoot)
        {
            _fileManager.EnsureDirectory(outputRoot);

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var outputDirectory = outputRoot;
                var @namespace = sourceCodeExpression.Namespace;

                if (!string.IsNullOrEmpty(@namespace))
                {
                    outputDirectory = Path.Combine(new[] { outputDirectory }
                        .Concat(@namespace.Split('.'))
                        .ToArray());

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
