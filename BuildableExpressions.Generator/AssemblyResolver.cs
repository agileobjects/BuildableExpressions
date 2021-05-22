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

        private static readonly string _sharedPath = 
            Path.Combine(Path.GetDirectoryName(_installPath)!, "shared");

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

            if (TryFindAssembly(_installPath, assemblyName, out var assembly))
            {
                return assembly;
            }

            if (TryFindAssembly(_sharedPath, assemblyName, out assembly))
            {
                return assembly;
            }

            return null;
        }

        private bool TryFindAssembly(string searchPath, string assemblyName, out Assembly assembly)
        {
            _logger.Info($"Looking for assembly '{assemblyName}' in {_installPath}...");

            var assemblyPath = _fileManager
                .FindFiles(searchPath, assemblyName)
                .FirstOrDefault();

            if (assemblyPath == null)
            {
                assembly = null;
                return false;
            }

            _logger.Info($"Loading assembly '{assemblyName}' from {_installPath}");
            assembly = Assembly.LoadFrom(assemblyPath);
            return true;
        }
    }
}
