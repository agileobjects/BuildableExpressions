namespace AgileObjects.BuildableExpressions.Generator.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Configuration;
    using Logging;
    using NetStandardPolyfills;
    using static System.IO.Path;
    using static System.Text.RegularExpressions.RegexOptions;

    internal class InputFilesFinder
    {
        public const string DefaultInputFileNamespace = "DefaultNamespace";
        public const string DefaultInputFileName = "ExpressionBuilder.cs";

        private const string _directoryCharacters = @"[\\/]+";

        private static readonly Regex _binOrObjPathMatcher = new(
            _directoryCharacters + "(?:bin|obj)" + _directoryCharacters,
            Compiled | IgnoreCase);

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        public InputFilesFinder(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;
        }

        public ICollection<InputFile> GetInputFiles(Config config)
        {
            var inputFiles = _fileManager
                .FindFiles(config.ContentRoot, "*.cs")
                .Where(cSharpFilePath => !_binOrObjPathMatcher.IsMatch(cSharpFilePath))
                .Select(cSharpFilePath => new InputFile
                {
                    FilePath = cSharpFilePath,
                    Contents = _fileManager.Read(cSharpFilePath)
                })
                .Where(_ =>
                    _.Contents.Contains(nameof(BuildableExpressions)) &&
                    _.Contents.Contains(nameof(ISourceCodeExpressionBuilder)))
                .Select(filePath =>
                {
                    _logger.Info($"Found Expression input file '{filePath}'");
                    return filePath;
                })
                .ToList();

            if (inputFiles.Any())
            {
                return inputFiles;
            }

            _logger.Info("Creating default input file...");

            var defaultInputFile = GetDefaultInputFile(config);

            _fileManager.Write(defaultInputFile.FilePath, defaultInputFile.Contents);

            return new[] { defaultInputFile };
        }

        private static InputFile GetDefaultInputFile(Config config)
        {
            var thisAssembly = typeof(InputFilesFinder).GetAssembly();

            var defaultInputFileResourceName = thisAssembly
                .GetManifestResourceNames()
                .First(name => name.EndsWith(DefaultInputFileName));

            var defaultInputFileResourceStream = thisAssembly
                .GetManifestResourceStream(defaultInputFileResourceName);

            using (defaultInputFileResourceStream)
            using (var fileStreamReader = new StreamReader(defaultInputFileResourceStream!))
            {
                var contents = fileStreamReader.ReadToEnd();

                if (config.RootNamespace != null)
                {
                    contents = contents
                        .Replace(DefaultInputFileNamespace, config.RootNamespace);
                }

                return new InputFile
                {
                    FilePath = Combine(config.ContentRoot, DefaultInputFileName),
                    Contents = contents
                };
            }
        }
    }
}
