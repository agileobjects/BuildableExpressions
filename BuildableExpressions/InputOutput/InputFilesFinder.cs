namespace AgileObjects.BuildableExpressions.InputOutput
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
    using static BuildConstants;

    internal class InputFilesFinder
    {
        private const string _directoryCharacter = @"[\\/]+";

        private static readonly Regex _binOrObjPathMatcher = new Regex(
            _directoryCharacter + "(?:bin|obj)" + _directoryCharacter,
            Compiled | IgnoreCase);

        private readonly IFileManager _fileManager;
        private readonly ILogger _logger;

        public InputFilesFinder(
            IFileManager fileManager,
            ILogger logger)
        {
            _fileManager = fileManager;
            _logger = logger;
        }

        public Config Config { get; set; }

        public ICollection<InputFile> GetInputFiles()
        {
            var inputFiles = _fileManager
                .FindFiles(Config.ContentRoot, "*.cs")
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

            if (!string.IsNullOrEmpty(Config.RootNamespace))
            {
                inputFileContent = inputFileContent
                    .Replace(DefaultInputFileNamespace, Config.RootNamespace);
            }

            var inputFilePath = Combine(Config.ContentRoot, DefaultInputFileName);

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
            using (var fileStreamReader = new StreamReader(defaultInputFileResourceStream))
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
