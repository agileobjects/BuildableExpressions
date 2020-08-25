namespace AgileObjects.BuildableExpressions.Configuration
{
    using System.IO;
    using static BuildConstants;

    internal class Config
    {
        private string _projectPath;
        private string _outputDirectory;
        private string _rootNamespace;
        private string _inputFile;

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                _projectPath = value;
                ContentRoot = Path.GetDirectoryName(value);
            }
        }

        public string ContentRoot { get; private set; }

        public string RootNamespace
        {
            get => _rootNamespace;
            set
            {
                _rootNamespace = string.IsNullOrEmpty(value)
                    ? Path.GetFileNameWithoutExtension(ProjectPath)
                    : value;
            }
        }

        public string InputFile
        {
            get
            {
                return string.IsNullOrEmpty(_inputFile)
                    ? (_inputFile = DefaultInputFile)
                    : _inputFile;
            }

            set => _inputFile = value;
        }

        public string OutputDirectory
        {
            get
            {
                return string.IsNullOrEmpty(_outputDirectory)
                    ? (_outputDirectory = DefaultOutputDirectory)
                    : _outputDirectory;
            }

            set => _outputDirectory = value;
        }

        public string OutputRoot
            => Path.Combine(ContentRoot, OutputDirectory);
    }
}
