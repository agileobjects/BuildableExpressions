// ReSharper disable once CheckNamespace
namespace BuildXpr
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AgileObjects.BuildableExpressions.Compilation;
    using AgileObjects.BuildableExpressions.Configuration;
    using AgileObjects.BuildableExpressions.InputOutput;
    using AgileObjects.BuildableExpressions.Logging;
    using AgileObjects.BuildableExpressions.ProjectManagement;
    using AgileObjects.BuildableExpressions.SourceCode;
    using MsBuildTask = Microsoft.Build.Utilities.Task;

    /// <summary>
    /// An MSBuild Task to generate source code files from <see cref="SourceCodeExpression"/>s.
    /// </summary>
    public class BuildExpressionsTask : MsBuildTask
    {
        private readonly ILogger _logger;
        private readonly InputFilesFinder _inputFilesFinder;
        private readonly OutputWriter _outputWriter;
        private readonly IProjectFactory _projectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildExpressionsTask"/> class.
        /// </summary>
        public BuildExpressionsTask()
            : this(
                MsBuildTaskLogger.Instance,
                new InputFilesFinder(
                    SystemIoFileManager.Instance,
                    MsBuildTaskLogger.Instance),
                new OutputWriter(SystemIoFileManager.Instance),
                new ProjectFactory(MsBuildTaskLogger.Instance, SystemIoFileManager.Instance))
        {
        }

        internal BuildExpressionsTask(
            ILogger logger,
            InputFilesFinder inputFilesFinder,
            OutputWriter outputWriter,
            IProjectFactory projectFactory)
        {
            _logger = logger.WithTask(this);
            _inputFilesFinder = inputFilesFinder;
            _outputWriter = outputWriter;
            _projectFactory = projectFactory;
        }

        /// <summary>
        /// Gets or sets the full path of the project providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the root namespace of the project providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Generates source code files from a set of <see cref="SourceCodeExpression"/>s.
        /// </summary>
        public override bool Execute()
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            try
            {
                var config = _inputFilesFinder.Config = _outputWriter.Config = new Config(ProjectPath)
                {
                    RootNamespace = RootNamespace
                };

                var project = _projectFactory.GetProject(config);

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
            compilationResult = CSharpCompiler.Compile(expressionBuilderSources);

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
