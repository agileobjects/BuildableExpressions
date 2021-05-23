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
                .ToList();

            if (inputFiles.Any())
            {
                return inputFiles;
            }

            _logger.Info("Creating default input file...");

            var inputFileContent = GetDefaultInputFile();

            if (config.RootNamespace != null)
            {
                inputFileContent = inputFileContent
                    .Replace(DefaultInputFileNamespace, config.RootNamespace);
            }

            var inputFilePath = Combine(config.ContentRoot, DefaultInputFileName);

            _fileManager.Write(inputFilePath, inputFileContent);

            return new[]
            {
                new InputFile
                {
                    FilePath = inputFilePath,
                    Contents = inputFileContent
                }
            };
        }

        private static string GetDefaultInputFile()
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
                return fileStreamReader.ReadToEnd();
            }
        }

        public class InputFile
        {
            public string FilePath { get; set; }

            public string Contents { get; set; }
        }
    }
}
