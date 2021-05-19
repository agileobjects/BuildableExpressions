namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using InputOutput;
    using Logging;

    internal class AssemblyResolver
    {
        private static readonly string _nuGetGlobalCacheFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget", "packages");

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        public AssemblyResolver(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;
        }

        public void Init(Config config)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                try
                {
                    var assemblyInfo = new AssemblyName(args.Name);
                    var packageName = assemblyInfo.Name;
                    var assemblyName = packageName + ".dll";
                    var version = assemblyInfo.Version.ToString(fieldCount: 3);

                    _logger.Warning($"Failed to resolve Assembly {assemblyInfo} - attempting manual resolve");

                    var packageDirectory = Path.Combine(_nuGetGlobalCacheFolder, packageName, version);

                    if (TryLoadAssembly(packageDirectory, assemblyName, out var assembly))
                    {
                        return assembly;
                    }

                    var packageDirectoryMatcher =
                        Path.Combine(_nuGetGlobalCacheFolder, packageName + ".*", version);

                    if (TryLoadAssembly(packageDirectoryMatcher, assemblyName, out assembly))
                    {
                        return assembly;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                return null;
            };
        }

        private bool TryLoadAssembly(
            string searchDirectory,
            string assemblyName,
            out Assembly assembly)
        {
            _logger.Info($"Looking for {assemblyName} in {searchDirectory}...");

            var assemblyPath = _fileManager
                .FindFiles(searchDirectory, assemblyName)
                .FirstOrDefault();

            if (assemblyPath == null)
            {
                assembly = null;
                return false;
            }

            assembly = Assembly.LoadFrom(assemblyPath);
            return true;
        }
    }
}
