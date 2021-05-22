namespace AgileObjects.BuildableExpressions.Generator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Compilation;
    using InputOutput;
    using Logging;
    using ProjectManagement;
    using SourceCode;

    internal class SourceCodeGenerator
    {
        private readonly ILogger _logger;
        private readonly InputFilesFinder _inputFilesFinder;
        private readonly OutputWriter _outputWriter;
        private readonly IProjectFactory _projectFactory;

        public SourceCodeGenerator(
            ILogger logger,
            InputFilesFinder inputFilesFinder,
            OutputWriter outputWriter,
            IProjectFactory projectFactory)
        {
            _logger = logger;
            _inputFilesFinder = inputFilesFinder;
            _outputWriter = outputWriter;
            _projectFactory = projectFactory;
        }

        public bool Execute()
        {
            try
            {
                var project = _projectFactory.GetProject();
                var inputFiles = _inputFilesFinder.GetInputFiles();

                _logger.Info("Compiling Expression files...");

                var compilationFailed = CompilationFailed(
                    inputFiles.Select(f => f.Contents),
                    out var compilationResult);

                if (compilationFailed)
                {
                    return false;
                }

                var sourceCodeExpressions = compilationResult.ToSourceCodeExpressions();
                var writtenFiles = _outputWriter.Write(sourceCodeExpressions);

                writtenFiles.Insert(0, Path.GetFileName(inputFiles.First().FilePath));

                project.Add(writtenFiles);

                _logger.Info("Expression compilation output updated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        private bool CompilationFailed(
            IEnumerable<string> expressionBuilderSources,
            out CompilationResult compilationResult)
        {
            compilationResult = CSharpCompiler.Compile(
                new[] { typeof(SourceCodeGenerator).Assembly },
                expressionBuilderSources);

            if (!compilationResult.Failed)
            {
                _logger.Info("Expression compilation succeeded");
                return false;
            }

            _logger.Error("Expression compilation failed:");

            foreach (var error in compilationResult.Errors)
            {
                _logger.Error(error);
            }

            return true;
        }
    }
}
