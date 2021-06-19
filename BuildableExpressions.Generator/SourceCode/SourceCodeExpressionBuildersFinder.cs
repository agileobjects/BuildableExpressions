namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Compilation;
    using Configuration;
    using Extensions;
    using Logging;
    using NetStandardPolyfills;
    using static Compilation.AssemblyResolver;

    internal class SourceCodeExpressionBuildersFinder
    {
        private readonly ILogger _logger;
        private readonly AssemblyResolver _assemblyResolver;
        private ICollection<Assembly> _projectAssemblies;

        public SourceCodeExpressionBuildersFinder(
            ILogger logger,
            AssemblyResolver assemblyResolver)
        {
            _logger = logger;
            _assemblyResolver = assemblyResolver;
        }

        public IEnumerable<Assembly> ProjectAssemblies => _projectAssemblies;

        public ICollection<ISourceCodeExpressionBuilder> Find(IConfig config)
        {
            var projectAssemblies = _assemblyResolver.LoadAssemblies(IsProjectAssembly);

            if (projectAssemblies.Count == 0)
            {
                return Array.Empty<ISourceCodeExpressionBuilder>();
            }

            var builders = EnumerateBuilders(projectAssemblies).ToList();

            if (builders.Any())
            {
                _projectAssemblies = projectAssemblies;
                return builders;
            }

            _logger.Warning(
                $"no {nameof(ISourceCodeExpressionBuilder)} implementations " +
                $"found in project {config.GetProjectNameWithoutExtension()}");

            return Array.Empty<ISourceCodeExpressionBuilder>();
        }

        private static bool IsProjectAssembly(AssemblyKey key)
        {
            if (key.AssemblyDirectory.EqualsIgnoreCase("ref"))
            {
                return false;
            }

            var assemblyFileName = key.AssemblyFileName;

            return
                assemblyFileName.DoesNotEqualIgnoreCase("AgileObjects.NetStandardPolyfills.dll") &&
                assemblyFileName.DoesNotEqualIgnoreCase("AgileObjects.ReadableExpressions.dll") &&
                assemblyFileName.DoesNotEqualIgnoreCase("AgileObjects.BuildableExpressions.dll") &&
                assemblyFileName.DoesNotEqualIgnoreCase("AgileObjects.AgileMapper.dll") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("System.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("Microsoft.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("EnvDTE") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("stdole") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("netstandard") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("Moq") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("Castle") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("nunit.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("xunit.") &&
                assemblyFileName.DoesNotStartWithIgnoreCase("Shouldy");
        }

        private IEnumerable<ISourceCodeExpressionBuilder> EnumerateBuilders(
            IEnumerable<Assembly> projectAssemblies)
        {
            foreach (var assembly in projectAssemblies)
            {
                ICollection<Type> builderTypes;

                try
                {
                    builderTypes = assembly
                        .GetAllTypes()
                        .Where(t => !t.IsAbstract() && t.IsAssignableTo(typeof(ISourceCodeExpressionBuilder)))
                        .ToList();
                }
                catch
                {
                    continue;
                }

                if (builderTypes.Count == 0)
                {
                    continue;
                }

                _logger.Info(
                    $"{builderTypes.Count} {nameof(ISourceCodeExpressionBuilder)}(s) " +
                    $"found in {assembly.GetName().Name}");

                foreach (var builderType in builderTypes)
                {
                    yield return (ISourceCodeExpressionBuilder)Activator.CreateInstance(builderType);
                }
            }
        }
    }
}