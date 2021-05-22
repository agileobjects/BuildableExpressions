namespace AgileObjects.BuildableExpressions.Generator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using InputOutput;
    using Logging;
    using NetStandardPolyfills;

    internal class AssemblyResolver
    {
        private static readonly string _installPath =
            Path.GetDirectoryName(typeof(AssemblyResolver).GetAssembly().GetLocation());

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        public AssemblyResolver(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;
        }

        public void Init()
            => AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyIfAvailable;

        private Assembly ResolveAssemblyIfAvailable(object sender, ResolveEventArgs args)
        {
            var assemblyInfo = new AssemblyName(args.Name);
            var assemblyName = assemblyInfo.Name + ".dll";

            _logger.Info($"Attempting to resolve assembly '{assemblyInfo}'...");
            _logger.Info($"Looking for assembly '{assemblyName}' in {_installPath}...");

            var packagedFilePath = _fileManager
                .FindFiles(_installPath, assemblyName)
                .FirstOrDefault();

            if (packagedFilePath != null)
            {
                return Assembly.LoadFrom(packagedFilePath);
            }

            return null;
        }
    }
}
