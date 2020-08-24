#if NET_STANDARD
namespace AgileObjects.BuildableExpressions.Configuration
{
    using System.IO;
    using InputOutput;
    using Microsoft.Extensions.Configuration;
    using static BuildConstants;

    internal class NetStandardConfigManager : IConfigManager
    {
        private readonly IFileManager _fileManager;

        public NetStandardConfigManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Populate(Config config)
        {
            var configFilePath = Path.Combine(config.ContentRoot, "appsettings.json");

            if (!_fileManager.Exists(configFilePath))
            {
                return;
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(config.ContentRoot)
                .AddJsonFile("appsettings.json", optional: false);

            var appSettings = builder.Build();

            config.InputFile = appSettings[$"appSettings:{InputFileKey}"];
            config.OutputDirectory = appSettings[$"appSettings:{OutputDirectoryKey}"];
        }
    }
}
#endif