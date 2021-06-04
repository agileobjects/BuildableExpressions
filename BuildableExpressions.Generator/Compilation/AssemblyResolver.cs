namespace AgileObjects.BuildableExpressions.Generator.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using InputOutput;
    using Logging;
    using NetStandardPolyfills;
    using static System.StringComparison;

    internal class AssemblyResolver
    {
        private static readonly string _installPath =
            Path.GetDirectoryName(typeof(AssemblyResolver).GetAssembly().GetLocation());

        private static readonly string _sharedPath =
            Path.Combine(Path.GetDirectoryName(_installPath)!, "shared");

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly List<Assembly> _outputAssemblies;
        private readonly object _outputAssembliesSync;

        public AssemblyResolver(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;
            _outputAssemblies = new List<Assembly>();
            _outputAssembliesSync = new object();

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyIfAvailable;
        }

        public IEnumerable<Assembly> GetReferenceAssemblies(IConfig config)
        {
            lock (_outputAssembliesSync)
            {
                if (!_outputAssemblies.Any())
                {
                    _outputAssemblies.AddRange(EnumerateReferenceAssemblies(config));
                }

                return _outputAssemblies;
            }
        }

        private IEnumerable<Assembly> EnumerateReferenceAssemblies(IConfig config)
        {
            var outputAssemblyPaths = _fileManager
                .FindFiles(config.GetOutputPath(), "*.dll");

            foreach (var assemblyPath in outputAssemblyPaths)
            {
                if (Ignore(assemblyPath))
                {
                    continue;
                }

                if (TryLoadAssembly(assemblyPath, out var assembly))
                {
                    yield return assembly;
                }
            }
        }

        private static bool Ignore(string assemblyPath)
        {
            var assemblyFileName = Path.GetFileName(assemblyPath);

            return 
                assemblyFileName.StartsWith("Microsoft.VisualStudio.", OrdinalIgnoreCase) ||
                assemblyFileName.StartsWith("nunit.", OrdinalIgnoreCase) ||
                assemblyFileName.StartsWith("xunit.", OrdinalIgnoreCase) ||
                assemblyFileName.StartsWith("Shouldy", OrdinalIgnoreCase);
        }

        private Assembly ResolveAssemblyIfAvailable(object sender, ResolveEventArgs args)
        {
            var assemblyInfo = new AssemblyName(args.Name);
            var assemblyName = assemblyInfo.Name + ".dll";

            _logger.Info($"Attempting to resolve assembly '{assemblyInfo}'...");

            var loadedAssembly = _outputAssemblies
                .FirstOrDefault(a => a.GetName().Name == assemblyInfo.Name);

            if (loadedAssembly != null)
            {
                _logger.Info($"Loaded assembly '{assemblyName}' from {loadedAssembly.GetLocation()}");
                return loadedAssembly;
            }

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
            _logger.Info($"Looking for assembly '{assemblyName}' in {searchPath}...");

            var assemblyPath = _fileManager
                .FindFiles(searchPath, assemblyName)
                .FirstOrDefault();

            if (assemblyPath != null)
            {
                return TryLoadAssembly(assemblyPath, out assembly);
            }

            assembly = null;
            return false;
        }

        private bool TryLoadAssembly(string assemblyPath, out Assembly assembly)
        {
            var assemblyFileName = Path.GetFileName(assemblyPath);

            try
            {
                assembly = Assembly.LoadFrom(assemblyPath);
                _logger.Info($"Loaded assembly '{assemblyFileName}' from {assemblyPath}");
                return true;
            }
            catch (Exception loadEx)
            {
                _logger.Warning(
                    $"Loading assembly '{assemblyFileName}' from {assemblyPath} FAILED: " +
                    loadEx.Message);

                assembly = null;
                return false;
            }
        }
    }
}
