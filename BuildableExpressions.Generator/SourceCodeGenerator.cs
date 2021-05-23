namespace AgileObjects.BuildableExpressions.Generator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Compilation;
    using Configuration;
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
            AssemblyResolver assemblyResolver,
            InputFilesFinder inputFilesFinder,
            OutputWriter outputWriter,
            IProjectFactory projectFactory)
        {
            _logger = logger;
            _inputFilesFinder = inputFilesFinder;
            _outputWriter = outputWriter;
            _projectFactory = projectFactory;

            assemblyResolver.Init();
        }

        public bool Execute(Config config)
        {
            try
            {
                var project = _projectFactory.GetProjectOrThrow(config);
                var inputFiles = _inputFilesFinder.GetInputFiles(config);

                _logger.Info("Compiling Expression files...");

                if (CompilationFailed(inputFiles, out var compilationResult))
                {
                    return false;
                }

                var sourceCodeExpressions = compilationResult.ToSourceCodeExpressions();
                var writtenFiles = _outputWriter.Write(config, sourceCodeExpressions);

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
            IEnumerable<InputFile> inputFiles,
            out CompilationResult compilationResult)
        {
            compilationResult = CSharpCompiler.Compile(
                new[] { typeof(SourceCodeGenerator).Assembly },
                inputFiles.Select(file => file.Contents));

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
