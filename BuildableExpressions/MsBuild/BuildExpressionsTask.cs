// ReSharper disable once CheckNamespace
namespace BuildExp
{
    using System;
    using System.IO;
    using AgileObjects.BuildableExpressions.Compilation;
    using AgileObjects.BuildableExpressions.Configuration;
    using AgileObjects.BuildableExpressions.InputOutput;
    using AgileObjects.BuildableExpressions.Logging;
    using AgileObjects.BuildableExpressions.SourceCode;
    using AgileObjects.NetStandardPolyfills;
    using static AgileObjects.BuildableExpressions.BuildConstants;
    using MsBuildTask = Microsoft.Build.Utilities.Task;

    /// <summary>
    /// An MSBuild Task to generate a source code file from a <see cref="SourceCodeExpression"/>.
    /// </summary>
    public class BuildExpressionsTask : MsBuildTask
    {
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly IConfigManager _configManager;
        private readonly ICompiler _compiler;
        private readonly OutputWriter _outputWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildExpressionsTask"/> class.
        /// </summary>
        public BuildExpressionsTask()
            : this(
                new MsBuildTaskLogger(),
                BclFileManager.Instance,
#if NETFRAMEWORK
                new NetFrameworkConfigManager(BclFileManager.Instance),
#else
                new NetStandardConfigManager(BclFileManager.Instance),
#endif
#if NETFRAMEWORK
                new NetFrameworkCompiler(),
#else
                new NetStandardCompiler(),
#endif
                new OutputWriter(BclFileManager.Instance))
        {
            ((MsBuildTaskLogger)_logger).SetTask(this);
        }

        internal BuildExpressionsTask(
            ILogger logger,
            IFileManager fileManager,
            IConfigManager configManager,
            ICompiler compiler,
            OutputWriter outputWriter)
        {
            _logger = logger;
            _fileManager = fileManager;
            _configManager = configManager;
            _compiler = compiler;
            _outputWriter = outputWriter;
        }

        /// <summary>
        /// Gets or sets the root path of the project providing the <see cref="SourceCodeExpression"/>
        /// to build.
        /// </summary>
        public string ContentRoot { get; set; }

        /// <summary>
        /// Generates a source code file from a <see cref="SourceCodeExpression"/>.
        /// </summary>
        public override bool Execute()
        {
            try
            {
                var config = _configManager.GetConfigOrNull(ContentRoot) ?? new Config();

                EnsureInputFile(config);

                _logger.Info($"Using input file {config.InputFile}");

                if (string.IsNullOrEmpty(config.OutputDirectory))
                {
                    config.OutputDirectory = DefaultOutputFile;
                }

                _logger.Info($"Using output file {config.OutputDirectory}");

                var expressionBuilderSource = _fileManager.Read(config.InputFile);

                var compilationResult = _compiler
                    .Compile(expressionBuilderSource);

                if (compilationResult.Failed)
                {
                    _logger.Error("Expression compilation failed:");

                    foreach (var error in compilationResult.Errors)
                    {
                        _logger.Error(error);
                    }

                    return false;
                }

                _logger.Info("Expression compilation succeeded");

                var sourceCodeExpressions = compilationResult.ToSourceCodeExpressions();

                _outputWriter.Write(
                    sourceCodeExpressions,
                    ContentRoot,
                    config);

                _logger.Info("Expression compilation output updated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        private void EnsureInputFile(Config config)
        {
            if (string.IsNullOrEmpty(config.InputFile))
            {
                config.InputFile = DefaultInputFile;
            }

            if (_fileManager.Exists(config.InputFile))
            {
                return;
            }

            _logger.Info($"Creating default input file {config.InputFile}...");

            var inputFilePath = Path.Combine(
                typeof(BuildExpressionsTask).GetAssembly().Location,
                "../../..",
                "content",
                config.InputFile);

            var inputFileContent = _fileManager.Read(inputFilePath);

            _fileManager.Write(Path.Combine(ContentRoot, config.InputFile), inputFileContent);
        }
    }
}
