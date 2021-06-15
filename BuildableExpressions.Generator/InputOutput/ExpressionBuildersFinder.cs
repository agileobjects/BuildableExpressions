namespace AgileObjects.BuildableExpressions.Generator.InputOutput
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

    internal class ExpressionBuildersFinder : IExpressionBuildContext
    {
        private readonly ILogger _logger;
        private readonly AssemblyResolver _assemblyResolver;
        private ICollection<Assembly> _projectAssemblies;

        public ExpressionBuildersFinder(
            ILogger logger,
            AssemblyResolver assemblyResolver)
        {
            _logger = logger;
            _assemblyResolver = assemblyResolver;
        }

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

            var assemblyName = key.AssemblyName;

            return
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.NetStandardPolyfills") &&
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.ReadableExpressions") &&
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.BuildableExpressions") &&
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.AgileMapper") &&
                assemblyName.DoesNotStartWithIgnoreCase("System.") &&
                assemblyName.DoesNotStartWithIgnoreCase("Microsoft.") &&
                assemblyName.DoesNotStartWithIgnoreCase("EnvDTE") &&
                assemblyName.DoesNotStartWithIgnoreCase("stdole") &&
                assemblyName.DoesNotStartWithIgnoreCase("netstandard") &&
                assemblyName.DoesNotStartWithIgnoreCase("Moq") &&
                assemblyName.DoesNotStartWithIgnoreCase("Castle");
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

        IEnumerable<Assembly> IExpressionBuildContext.ProjectAssemblies
            => _projectAssemblies;

        public void Log(string message) => _logger.Info(message);

        public void Log(Exception exception) => _logger.Error(exception);
    }
}