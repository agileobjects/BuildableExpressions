namespace AgileObjects.BuildableExpressions.Generator.Compilation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    //using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Extensions;
    using InputOutput;
    using Logging;
    using static System.IO.Path;
    using static BuildableExpressions.Compilation.CSharpCompiler;

    internal class AssemblyResolver
    {
        private static readonly string _packageInstallPath;
        private static readonly string _packageSharedPath;
        private static readonly string _packagesRootPath;
        private static readonly string[] _frameworkAssemblyNames;

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly ConcurrentDictionary<AssemblyKey, Lazy<Assembly>> _assemblyLoadersByName;

        public AssemblyResolver(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;

            _assemblyLoadersByName =
                new ConcurrentDictionary<AssemblyKey, Lazy<Assembly>>(AssemblyKeyComparer.Default);

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyIfAvailable;
        }

        static AssemblyResolver()
        {
            _packageInstallPath = GetDirectoryName(typeof(AssemblyResolver).Assembly.Location);
            var packageToolsPath = GetDirectoryName(_packageInstallPath);
            _packageSharedPath = Combine(packageToolsPath!, "shared");

            var packageRootPath = packageToolsPath;

            while (GetFileName(packageRootPath).DoesNotStartWithIgnoreCase("AgileObjects."))
            {
                packageRootPath = GetDirectoryName(packageRootPath);
            }

            _packagesRootPath = GetDirectoryName(packageRootPath);

            _frameworkAssemblyNames = new[]
            {
                "AgileObjects.NetStandardPolyfills.dll",
                "AgileObjects.ReadableExpressions.dll",
                "AgileObjects.BuildableExpressions.dll"
            };
        }

        #region Setup

        public void Init(IConfig config)
        {
            PopulateFrameworkAssemblyLoaders(config);
            PopulateAssemblyLoadersFromInput(config);
        }

        private void PopulateFrameworkAssemblyLoaders(IConfig config)
        {
            var frameworkDirectories = _fileManager
                .FindDirectories(_packagesRootPath, "AgileObjects.*", recursive: false)
                .ToList();

            var targetFramework = config.TargetFramework;

            foreach (var frameworkAssemblyName in _frameworkAssemblyNames)
            {
                var frameworkAssemblyPath = frameworkDirectories
                    .SelectMany(directory =>
                    {
                        var frameworkDirectory = _fileManager
                            .FindDirectories(directory, targetFramework, recursive: true)
                            .First();

                        return _fileManager
                            .FindFiles(frameworkDirectory, frameworkAssemblyName);
                    })
                    .FirstOrDefault();

                if (frameworkAssemblyPath != null)
                {
                    AddAssemblyLoader(frameworkAssemblyPath, Assembly.LoadFrom);
                }
            }
        }

        private void PopulateAssemblyLoadersFromInput(IConfig config)
        {
            foreach (var assemblyPath in EnumerateInputAssemblyPaths(config))
            {
                AddAssemblyLoader(assemblyPath, LoadAssembly);
            }
        }

        private IEnumerable<string> EnumerateInputAssemblyPaths(IConfig config)
        {
            var inputPath = config.GetInputPath();

            var inputAssemblyPaths = _fileManager
                .FindFiles(inputPath, "*.dll");

            var inputExePaths = _fileManager
                .FindFiles(inputPath, "*.exe");

            return inputAssemblyPaths
                .Concat(inputExePaths)
                .Where(IncludeOutputAssembly);
        }

        private static bool IncludeOutputAssembly(string assemblyPath)
        {
            var assemblyFileName = GetFileName(assemblyPath);

            return
                assemblyFileName.DoesNotStartWithIgnoreCase("Microsoft.VisualStudio.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("nunit.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("xunit.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("Shouldy");
        }

        private void AddAssemblyLoader(string assemblyPath, Func<string, Assembly> loader)
        {
            _assemblyLoadersByName.TryAdd(
                new AssemblyKey(_fileManager, assemblyPath),
                new Lazy<Assembly>(() => loader.Invoke(assemblyPath)));
        }

        #endregion

        public ICollection<Assembly> LoadAssemblies(Func<AssemblyKey, bool> matcher)
        {
            if (TryLoadAssemblies(matcher, out var projectAssemblies))
            {
                return projectAssemblies;
            }

            return Array.Empty<Assembly>();
        }

        private bool TryLoadAssemblies(
            Func<AssemblyKey, bool> matcher,
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
            Func<AssemblyKey, bool> matcher,
            out ICollection<Lazy<Assembly>> assemblyLoaders)
        {
            assemblyLoaders = _assemblyLoadersByName
                .Where(kvp => matcher.Invoke(kvp.Key))
                .Select(kvp => kvp.Value)
                .ToList();

            return assemblyLoaders.Any();
        }

        private Assembly ResolveAssemblyIfAvailable(object sender, ResolveEventArgs args)
        {
            var assemblyInfo = new AssemblyName(args.Name);
            var assemblyDllName = assemblyInfo.Name + ".dll";

            if (TryFindAssemblyLoaders(key => Matches(assemblyDllName, key), out var loaders) &&
                loaders.First().IsValueCreated)
            {
                return loaders.First().Value;
            }

            if (TryFindAssembly(_packageInstallPath, assemblyDllName, out var assembly))
            {
                return assembly;
            }

            if (TryFindAssembly(_packageSharedPath, assemblyDllName, out assembly))
            {
                return assembly;
            }

            if (TryLoadAssemblies(key => Matches(assemblyDllName, key), out var assemblies))
            {
                return assemblies.First();
            }

            return null;
        }

        private static bool Matches(string assemblyDllName, AssemblyKey key)
            => assemblyDllName.EqualsIgnoreCase(key.AssemblyName);

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
                var assembly = assemblyPath.StartsWithIgnoreCase(_packagesRootPath)
                    ? Assembly.LoadFrom(assemblyPath)
                    : LoadAssemblyFromCopy(assemblyPath);

                if (assembly != null)
                {
                    _logger.Info($"loaded assembly '{assemblyFileName}' from {assemblyFolderName}");
                }

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

        private Assembly LoadAssemblyFromCopy(string assemblyPath)
        {
            //using var fileStream = _fileManager.OpenRead(assemblyPath);
            //using var assemblyStream = new MemoryStream();

            //fileStream.CopyTo(assemblyStream);
            //return Assembly.Load(assemblyStream.ToArray());

            var assemblyDirectory = GetDirectoryName(assemblyPath);
            var assemblyName = GetFileNameWithoutExtension(assemblyPath);
            var extension = GetExtension(assemblyPath);

            if (extension.EqualsIgnoreCase(".exe"))
            {
                var matchingDllExists = _fileManager
                    .FindFiles(assemblyDirectory, assemblyName + ".dll")
                    .Any();

                if (matchingDllExists)
                {
                    _logger.Info($"skipping '{assemblyName}.exe' as matching DLL exists");
                    return null;
                }
            }

            var copyAssemblyPath = GetTempAssemblyPath(assemblyName + "_", extension);

            CopyFile(assemblyPath, copyAssemblyPath);
            return Assembly.LoadFrom(copyAssemblyPath);
        }

        private void CopyFile(string sourcePath, string targetPath)
        {
            using var assemblyReader = _fileManager.OpenRead(sourcePath);
            using var assemblyWriter = _fileManager.OpenWrite(targetPath);

            assemblyReader.CopyTo(assemblyWriter);
        }

        #region Helper Class

        public class AssemblyKey
        {
            private readonly Lazy<string> _versionLoader;

            public AssemblyKey(
                IFileManager fileManager,
                string assemblyPath)
            {
                AssemblyName = GetFileName(assemblyPath);
                AssemblyDirectory = GetFileName(GetDirectoryName(assemblyPath));
                _versionLoader = new Lazy<string>(() => fileManager.GetVersion(assemblyPath));
            }

            public string AssemblyName { get; }

            public string AssemblyDirectory { get; }

            public string Version => _versionLoader.Value;

            public override string ToString() => AssemblyName;
        }

        private class AssemblyKeyComparer : IEqualityComparer<AssemblyKey>
        {
            public static readonly IEqualityComparer<AssemblyKey> Default =
                new AssemblyKeyComparer();

            public bool Equals(AssemblyKey x, AssemblyKey y)
            {
                return x!.AssemblyName.EqualsIgnoreCase(y!.AssemblyName) &&
                       x.Version == y.Version;
            }

            public int GetHashCode(AssemblyKey obj) => 0;
        }

        #endregion
    }
}
