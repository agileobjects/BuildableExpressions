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
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(assemblyPath);
                    
                    _logger.Info(
                        $"Loaded assembly '{assembly.FullName}' " +
                        $"from {Path.GetDirectoryName(assemblyPath)}");
                }
                catch
                {
                    continue;
                }

                yield return assembly;
            }
        }

        private Assembly ResolveAssemblyIfAvailable(object sender, ResolveEventArgs args)
        {
            var assemblyInfo = new AssemblyName(args.Name);
            var assemblyName = assemblyInfo.Name + ".dll";

            _logger.Info($"Attempting to resolve assembly '{assemblyInfo}'...");

            var loadedAssembly = _outputAssemblies
                .FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (loadedAssembly != null)
            {
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
