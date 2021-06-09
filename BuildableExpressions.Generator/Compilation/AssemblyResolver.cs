namespace AgileObjects.BuildableExpressions.Generator.Compilation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Extensions;
    using InputOutput;
    using Logging;
    using static System.IO.Path;

    internal class AssemblyResolver
    {
        private static readonly string _installPath = 
            GetDirectoryName(typeof(AssemblyResolver).Assembly.Location);

        private static readonly string _installRootPath = GetDirectoryName(_installPath);
        private static readonly string _sharedPath = Combine(_installRootPath!, "shared");

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly ConcurrentDictionary<string, Lazy<Assembly>> _assemblyLoadersByName;

        public AssemblyResolver(
            ILogger logger, 
            IFileManager fileManager,
            IConfig config)
        {
            _logger = logger;
            _fileManager = fileManager;
            _assemblyLoadersByName = new ConcurrentDictionary<string, Lazy<Assembly>>();

            PopulateAssemblyLoadersFromOutput(config);
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyIfAvailable;
        }

        private void PopulateAssemblyLoadersFromOutput(IConfig config)
        {
            foreach (var assemblyPath in EnumerateOutputAssemblyPaths(config))
            {
                AddAssemblyLoader(assemblyPath, LoadAssembly);
            }
        }

        private IEnumerable<string> EnumerateOutputAssemblyPaths(IConfig config)
        {
            var outputPath = config.GetOutputPath();

            var outputAssemblyPaths = _fileManager
                .FindFiles(outputPath, "*.dll");

            var outputExePaths = _fileManager
                .FindFiles(outputPath, "*.exe");

            return outputAssemblyPaths
                .Concat(outputExePaths)
                .Where(IncludeOutputAssembly);
        }

        private static bool IncludeOutputAssembly(string assemblyPath)
        {
            var assemblyFileName = GetFileName(assemblyPath);

            return
                assemblyFileName.DoesNotStartWithIgnoreCase("Microsoft.VisualStudio.") ||
                assemblyFileName.DoesNotStartWithIgnoreCase("nunit.") ||
                assemblyFileName.DoesNotStartWithIgnoreCase("xunit.") ||
                assemblyFileName.DoesNotStartWithIgnoreCase("Shouldy");
        }

        private void AddAssemblyLoader(string assemblyPath, Func<string, Assembly> loader)
        {
            _assemblyLoadersByName.TryAdd(
                assemblyPath,
                new Lazy<Assembly>(() => loader.Invoke(assemblyPath)));
        }

        public ICollection<Assembly> LoadAssemblies(Func<string, bool> matcher)
        {
            if (TryLoadAssemblies(matcher, out var projectAssemblies))
            {
                return projectAssemblies;
            }

            return Array.Empty<Assembly>();
        }

        private bool TryLoadAssemblies(
            Func<string, bool> matcher,
            out ICollection<Assembly> assemblies)
        {
            if (TryFindAssemblyLoaders(matcher, out var loaders))
            {
                assemblies = loaders
                    .Select(l => l.Value)
                    .Where(assembly => assembly != null)
                    .ToList();

                return assemblies.Any();
            }

            assemblies = Array.Empty<Assembly>();
            return false;
        }

        private bool TryFindAssemblyLoaders(
            Func<string, bool> matcher,
            out ICollection<Lazy<Assembly>> assemblyLoaders)
        {
            assemblyLoaders = _assemblyLoadersByName
                .Where(kvp => matcher.Invoke(GetFileNameWithoutExtension(kvp.Key)))
                .Select(kvp => kvp.Value)
                .ToList();

            return assemblyLoaders.Any();
        }

        private Assembly ResolveAssemblyIfAvailable(object sender, ResolveEventArgs args)
        {
            var assemblyInfo = new AssemblyName(args.Name);
            var assemblyName = assemblyInfo.Name;
            var assemblyDllName = assemblyName + ".dll";

            if (TryFindAssemblyLoaders(name => name == assemblyName, out var loaders) &&
                loaders.First().IsValueCreated)
            {
                return loaders.First().Value;
            }

            if (TryFindAssembly(_installPath, assemblyDllName, out var assembly))
            {
                return assembly;
            }

            if (TryFindAssembly(_sharedPath, assemblyDllName, out assembly))
            {
                return assembly;
            }

            if (TryLoadAssemblies(name => name == assemblyName, out var assemblies))
            {
                return assemblies.First();
            }

            return null;
        }

        private bool TryFindAssembly(string searchPath, string assemblyName, out Assembly assembly)
        {
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
            assembly = LoadAssembly(assemblyPath);

            if (assembly == null)
            {
                return false;
            }

            var loadedAssembly = assembly;
            AddAssemblyLoader(assemblyPath, _ => loadedAssembly);
            return true;
        }

        private Assembly LoadAssembly(string assemblyPath)
        {
            var assemblyFileName = GetFileName(assemblyPath);
            var assemblyFolderName = GetDirectoryName(assemblyPath);

            try
            {
                var assembly = assemblyPath.StartsWithIgnoreCase(_installRootPath)
                    ? Assembly.LoadFrom(assemblyPath)
                    : LoadAssemblyFromInMemoryCopy(assemblyPath);

                _logger.Info($"loaded assembly '{assemblyFileName}' from {assemblyFolderName}");
                return assembly;
            }
            catch (Exception loadEx)
            {
                _logger.Warning(
                    $"loading assembly '{assemblyFileName}' from {assemblyFolderName} FAILED: " +
                    loadEx.Message);

                return null;
            }
        }

        private Assembly LoadAssemblyFromInMemoryCopy(string assemblyPath)
        {
            using var fileStream = _fileManager.OpenRead(assemblyPath);
            using var assemblyStream = new MemoryStream();

            fileStream.CopyTo(assemblyStream);
            return Assembly.Load(assemblyStream.ToArray());
        }
    }
}
