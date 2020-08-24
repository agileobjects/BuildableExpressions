#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Configuration
{
    using System.Configuration;
    using System.IO;
    using InputOutput;
    using static BuildConstants;

    internal class NetFrameworkConfigManager : IConfigManager
    {
        private readonly IFileManager _fileManager;

        public NetFrameworkConfigManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Populate(Config config)
        {
            var configFilePath = Path.Combine(config.ContentRoot, "web.config");

            if (_fileManager.Exists(configFilePath))
            {
                PopulateFrom(configFilePath, config);
                return;
            }

            configFilePath = Path.Combine(config.ContentRoot, "app.config");

            if (_fileManager.Exists(configFilePath))
            {
                PopulateFrom(configFilePath, config);
            }
        }

        private static void PopulateFrom(string configFilePath, Config config)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(configFilePath);

            config.InputFile = exeConfig.AppSettings.Settings[InputFileKey]?.Value;
            config.OutputDirectory = exeConfig.AppSettings.Settings[OutputDirectoryKey]?.Value;
        }
    }
}
#endif