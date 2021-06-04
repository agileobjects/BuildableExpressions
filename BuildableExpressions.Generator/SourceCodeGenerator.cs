namespace AgileObjects.BuildableExpressions.Generator
{
    using System;
    using System.Linq;
    using BuildableExpressions.Compilation;
    using Compilation;
    using Configuration;
    using InputOutput;
    using Logging;
    using ProjectManagement;
    using SourceCode;

    internal class SourceCodeGenerator
    {
        private readonly ILogger _logger;
        private readonly AssemblyResolver _assemblyResolver;
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
            _assemblyResolver = assemblyResolver;
            _inputFilesFinder = inputFilesFinder;
            _outputWriter = outputWriter;
            _projectFactory = projectFactory;
        }

        public bool Execute(IConfig config)
        {
            try
            {
                var project = _projectFactory.GetProjectOrThrow(config);

                _logger.Info("Compiling Expression files...");

                if (CompilationFailed(config, out var compilationResult))
                {
                    return false;
                }

                var sourceCodeExpressions = compilationResult.ToSourceCodeExpressions();
                var writtenFiles = _outputWriter.Write(config, sourceCodeExpressions);

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
            IConfig config,
            out CompilationResult compilationResult)
        {
            var referenceAssemblies = _assemblyResolver.GetReferenceAssemblies(config);
            var inputFiles = _inputFilesFinder.GetInputFiles(config);

            compilationResult = CSharpCompiler.Compile(
                referenceAssemblies,
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
