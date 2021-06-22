namespace AgileObjects.BuildableExpressions.Generator.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Extensions;
    using InputOutput;
    using Logging;
    using static System.IO.Path;
    using static BuildableExpressionExtensions;

    internal class AssemblyResolver
    {
        private static readonly string _packageInstallPath;
        private static readonly string _packageSharedPath;
        private static readonly string _packagesRootPath;
        private static readonly string[] _frameworkAssemblyNames;

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly ConcurrentDictionary<AssemblyKey, Lazy<Assembly>> _assemblyLoaders;

        public AssemblyResolver(ILogger logger, IFileManager fileManager)
        {
            _logger = logger;
            _fileManager = fileManager;

            _assemblyLoaders =
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
                .Where(IncludeInputAssembly);
        }

        private static bool IncludeInputAssembly(string assemblyPath)
        {
            var assemblyFileName = GetFileName(assemblyPath);

            return
                assemblyFileName.DoesNotStartWithIgnoreCase("Microsoft.VisualStudio.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("testhost.");
        }

        private void AddAssemblyLoader(Assembly loadedAssembly)
        {
            _assemblyLoaders[new AssemblyKey(loadedAssembly)] =
                new Lazy<Assembly>(() => loadedAssembly);
        }

        private void AddAssemblyLoader(string assemblyPath, Func<string, Assembly> loader)
        {
            _assemblyLoaders.TryAdd(
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
            assemblyLoaders = _assemblyLoaders
                .Where(kvp => matcher.Invoke(kvp.Key))
                .Select(kvp => kvp.Value)
                .ToList();

            return assemblyLoaders.Any();
        }

        private Assembly ResolveAssemblyIfAvailable(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(AppDomain.CurrentDomain.ApplyPolicy(args.Name));
            var assemblyFileName = assemblyName.Name + ".dll";

            if (TryFindAssemblyLoaders(k => k.Matches(assemblyFileName, assemblyName.Version), out var loaders) &&
                loaders.First().IsValueCreated)
            {
                return loaders.First().Value;
            }

            if (TryFindPackageAssembly(_packageInstallPath, assemblyFileName, out var assembly))
            {
                return assembly;
            }

            if (TryFindPackageAssembly(_packageSharedPath, assemblyFileName, out assembly))
            {
                return assembly;
            }

            if (TryLoadAssemblies(k => k.Matches(assemblyFileName, assemblyName.Version), out var assemblies))
            {
                return assemblies.First();
            }

            // Fallback to a loaded assembly with the same name - if this
            // matches an assembly it'll be an older version, but it may
            // still work and that's better than falling over:
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(asm => asm.GetName().Name == assemblyName.Name);
        }

        private bool TryFindPackageAssembly(
            string searchPath,
            string assemblyName,
            out Assembly assembly)
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

            AddAssemblyLoader(assembly);
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

            var copyAssemblyPath = GetTempCopyFilePath(assemblyName, extension);

            CopyFile(assemblyPath, copyAssemblyPath);
            return Assembly.LoadFrom(copyAssemblyPath);
        }

        private void CopyFile(string sourcePath, string targetPath)
        {
            using var assemblyReader = _fileManager.OpenRead(sourcePath);
            using var assemblyWriter = _fileManager.OpenWrite(targetPath);

            assemblyReader.CopyTo(assemblyWriter);
        }

        #region Helper Classes

        public enum AssemblySource
        {
            Package,
            PackageShared,
            Packages,
            Output
        }

        public class AssemblyKey
        {
            private readonly Lazy<Version> _versionLoader;

            public AssemblyKey(Assembly assembly)
                : this(assembly.Location)
            {
                _versionLoader = new Lazy<Version>(() => assembly.GetName().Version);
            }

            public AssemblyKey(IFileManager fileManager, string assemblyPath)
                : this(assemblyPath)
            {
                _versionLoader = new Lazy<Version>(() => fileManager.GetVersion(assemblyPath));
            }

            private AssemblyKey(string assemblyPath)
            {
                AssemblyFileName = GetFileName(assemblyPath);
                AssemblyDirectory = GetFileName(GetDirectoryName(assemblyPath));

                Source = assemblyPath.StartsWithIgnoreCase(_packageInstallPath)
                    ? AssemblySource.Package
                    : assemblyPath.StartsWithIgnoreCase(_packageSharedPath)
                        ? AssemblySource.PackageShared
                        : assemblyPath.StartsWithIgnoreCase(_packagesRootPath)
                            ? AssemblySource.Packages
                            : AssemblySource.Output;
            }

            public AssemblySource Source { get; }

            public string AssemblyFileName { get; }

            public string AssemblyDirectory { get; }

            public Version Version => _versionLoader.Value;

            public bool Matches(string assemblyFileName, Version minimumVersion)
                => Matches(assemblyFileName) && Version >= minimumVersion;

            public bool Matches(string assemblyFileName)
                => AssemblyFileName.EqualsIgnoreCase(assemblyFileName);

            public override string ToString() => $"{AssemblyFileName} ({Source})";
        }

        private class AssemblyKeyComparer : IEqualityComparer<AssemblyKey>
        {
            public static readonly IEqualityComparer<AssemblyKey> Default =
                new AssemblyKeyComparer();

            public bool Equals(AssemblyKey x, AssemblyKey y)
                => x!.Matches(y!.AssemblyFileName) && x.Version == y.Version;

            public int GetHashCode(AssemblyKey obj) => 0;
        }

        #endregion
    }
}
