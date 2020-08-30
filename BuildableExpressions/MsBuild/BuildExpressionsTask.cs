// ReSharper disable once CheckNamespace
namespace BuildXpr
{
    using System;
    using System.Collections.Generic;
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
    public class BuildExpressionsTask : MsBuildTask, IRootNamespaceAccessor
    {
        private readonly ILogger _logger;
        private readonly InputFilesFinder _inputFilesFinder;
        private readonly ICompiler _compiler;
        private readonly IRootNamespaceAccessor _rootNamespaceAccessor;
        private readonly OutputWriter _outputWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildExpressionsTask"/> class.
        /// </summary>
        public BuildExpressionsTask()
            : this(
                MsBuildTaskLogger.Instance,
                new InputFilesFinder(
                    BclFileManager.Instance,
                    MsBuildTaskLogger.Instance),
#if NETFRAMEWORK
                new NetFrameworkCompiler(),
#else
                new NetStandardCompiler(),
#endif
                new OutputWriter(
                    BclFileManager.Instance,
#if NETFRAMEWORK
                    new NetFrameworkProjectManager(BclFileManager.Instance)
#else
                    new NullProjectManager()
#endif
                    ))
        {
        }

        internal BuildExpressionsTask(
            ILogger logger,
            InputFilesFinder inputFilesFinder,
            ICompiler compiler,
            OutputWriter outputWriter)
        {
            _logger = logger.WithTask(this);
            _inputFilesFinder = inputFilesFinder;
            _compiler = compiler;
            _outputWriter = outputWriter;
#if  NETFRAMEWORK
            _rootNamespaceAccessor = outputWriter.ProjectManager;
#else
            _rootNamespaceAccessor = this;
#endif
        }

        /// <summary>
        /// Gets or sets the full path of the project providing the <see cref="SourceCodeExpression"/>
        /// to build.
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the root path of the project providing the <see cref="SourceCodeExpression"/>
        /// to build.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Generates source code files from a set of <see cref="SourceCodeExpression"/>s.
        /// </summary>
        public override bool Execute()
        {
            try
            {
                var config = new Config
                {
                    ProjectPath = ProjectPath,
                    RootNamespace = _rootNamespaceAccessor.RootNamespace
                };

                var inputFiles = _inputFilesFinder.GetInputFiles(config);
                var sourceCodeExpressions = new List<SourceCodeExpression>();

                foreach (var inputFile in inputFiles)
                {
                    _logger.Info($"Compiling Expressions from {inputFile.FilePath} to {config.OutputRoot}...");

                    var compilationFailed = _compiler.CompilationFailed(
                        inputFile.Contents,
                        _logger,
                        out var compilationResult);

                    if (compilationFailed)
                    {
                        return false;
                    }

                    _logger.Info("Expression compilation succeeded");

                    sourceCodeExpressions.AddRange(compilationResult.ToSourceCodeExpressions());
                }

                _outputWriter.Write(sourceCodeExpressions, config);

                _logger.Info("Expression compilation output updated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
    }
}
