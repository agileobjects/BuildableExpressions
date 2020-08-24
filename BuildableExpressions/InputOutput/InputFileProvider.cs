namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.IO;
    using BuildXpr;
    using Configuration;
    using Logging;
    using NetStandardPolyfills;
    using static BuildConstants;

    internal class InputFileProvider
    {
        private readonly IFileManager _fileManager;
        private readonly ILogger _logger;

        public InputFileProvider(
            IFileManager fileManager,
            ILogger logger)
        {
            _fileManager = fileManager;
            _logger = logger;
        }

        public void EnsureInputFile(Config config)
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
                "..", "..", "..",
                "content",
                config.InputFile);

            var inputFileContent = _fileManager.Read(inputFilePath);

            if (!string.IsNullOrEmpty(config.RootNamespace))
            {
                inputFileContent = inputFileContent
                    .Replace(DefaultInputFileNamespace, config.RootNamespace);
            }

            _fileManager.Write(
                Path.Combine(config.ContentRoot, config.InputFile),
                inputFileContent);
        }
    }
}
