namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Configuration;
    using ProjectManagement;
    using SourceCode;
    using static System.StringComparison;

    internal class OutputWriter
    {
        private readonly IFileManager _fileManager;

        public OutputWriter(
            IFileManager fileManager,
            IProjectManager projectManager)
        {
            _fileManager = fileManager;
            ProjectManager = projectManager;
        }

        public IProjectManager ProjectManager { get; }

        public void Write(
            IEnumerable<SourceCodeExpression> sourceCodeExpressions,
            Config config)
        {
            var rootNamespace = config.RootNamespace;

            foreach (var sourceCodeExpression in sourceCodeExpressions)
            {
                var @namespace = sourceCodeExpression.Namespace;

                var outputDirectory = config.ContentRoot;
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

                var fileName = sourceCodeExpression.Classes.First().Name + ".cs";
                var filePath = Path.Combine(outputDirectory, fileName);

                var sourceCode = sourceCodeExpression.ToSourceCode();

                _fileManager.Write(filePath, sourceCode);
                ProjectManager.Add(Path.Combine(relativeFilePath, fileName));
            }

            ProjectManager.Save();
        }
    }
}
