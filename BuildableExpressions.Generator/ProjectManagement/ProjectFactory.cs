namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System;
    using System.IO;
    using System.Linq;
    using Configuration;
    using InputOutput;
    using Logging;
    using static System.StringComparison;

    internal class ProjectFactory : IProjectFactory
    {
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly Config _config;

        public ProjectFactory(
            ILogger logger,
            IFileManager fileManager,
            Config config)
        {
            _logger = logger;
            _fileManager = fileManager;
            _config = config;
        }

        public IProject GetProject()
        {
            using var fileReadStream = _fileManager.OpenRead(_config.ProjectPath);
            using var fileReader = new StreamReader(fileReadStream);

            while (true)
            {
                var fileLine = fileReader.ReadLine();

                if (fileLine == null || char.IsWhiteSpace(fileLine.First()))
                {
                    var ex = new NotSupportedException(
                        $"Unable to find <Project /> element in file '{_config.ProjectPath}'");
                    
                    _logger.Error(ex);
                    throw ex;
                }

                if (!fileLine.StartsWith("<Project", OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(_config.RootNamespace))
                {
                    TryPopulateRootNamespace(_config, fileReader);
                }

#if NETFRAMEWORK
                if (fileLine.IndexOf(" Sdk=\"", OrdinalIgnoreCase) != -1)
                {
                    return new SdkProject();
                }

                return new NetFrameworkProject(_config);
#else
                return new SdkProject();
#endif
            }
        }

        private static void TryPopulateRootNamespace(Config config, TextReader fileReader)
        {
            string fileLine;

            while ((fileLine = fileReader.ReadLine()?.TrimStart()) != null)
            {
                if (!fileLine.StartsWith("<RootNamespace>", OrdinalIgnoreCase))
                {
                    continue;
                }

                var @namespace = fileLine
                    .Substring("<RootNamespace>".Length)
                    .Split('<')[0];

                config.RootNamespace = @namespace;
                return;
            }
        }
    }
}