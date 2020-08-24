// ReSharper disable once CheckNamespace
namespace BuildXpr
{
    using System;
    using AgileObjects.BuildableExpressions.Compilation;
    using AgileObjects.BuildableExpressions.Configuration;
    using AgileObjects.BuildableExpressions.InputOutput;
    using AgileObjects.BuildableExpressions.Logging;
    using AgileObjects.BuildableExpressions.ProjectManagement;
    using AgileObjects.BuildableExpressions.SourceCode;
    using MsBuildTask = Microsoft.Build.Utilities.Task;

    /// <summary>
    /// An MSBuild Task to generate a source code file from a <see cref="SourceCodeExpression"/>.
    /// </summary>
    public class BuildExpressionsTask : MsBuildTask
    {
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly IConfigManager _configManager;
        private readonly InputFileProvider _inputFileProvider;
        private readonly ICompiler _compiler;
        private readonly OutputWriter _outputWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildExpressionsTask"/> class.
        /// </summary>
        public BuildExpressionsTask()
            : this(
                MsBuildTaskLogger.Instance,
                BclFileManager.Instance,
#if NETFRAMEWORK
                new NetFrameworkConfigManager(BclFileManager.Instance),
#else
                new NetStandardConfigManager(BclFileManager.Instance),
#endif
                new InputFileProvider(
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
                    new NetFrameworkProjectManager()
#else
                    new NullProjectManager()
#endif
                    ))
        {
            MsBuildTaskLogger.Instance.SetTask(this);
        }

        internal BuildExpressionsTask(
            ILogger logger,
            IFileManager fileManager,
            IConfigManager configManager,
            InputFileProvider inputFileProvider,
            ICompiler compiler,
            OutputWriter outputWriter)
        {
            _logger = logger;
            _fileManager = fileManager;
            _configManager = configManager;
            _inputFileProvider = inputFileProvider;
            _compiler = compiler;
            _outputWriter = outputWriter;
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
                    RootNamespace = RootNamespace
                };

                _configManager.Populate(config);
                _inputFileProvider.EnsureInputFile(config);

                _logger.Info($"Compiling Expressions from {config.InputFile} to {config.OutputRoot}...");

                var expressionBuilderSource = _fileManager.Read(config.InputFile);

                var compilationFailed = _compiler.CompilationFailed(
                    expressionBuilderSource,
                    _logger,
                    out var compilationResult);

                if (compilationFailed)
                {
                    return false;
                }

                _logger.Info("Expression compilation succeeded");

                var sourceCodeExpressions = compilationResult.ToSourceCodeExpressions();

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
