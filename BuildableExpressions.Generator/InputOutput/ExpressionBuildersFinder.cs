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

    internal class ExpressionBuildersFinder
    {
        private readonly ILogger _logger;
        private readonly AssemblyResolver _assemblyResolver;

        public ExpressionBuildersFinder(
            ILogger logger,
            AssemblyResolver assemblyResolver)
        {
            _logger = logger;
            _assemblyResolver = assemblyResolver;
        }

        public ICollection<ISourceCodeExpressionBuilder> Find(IConfig config)
        {
            _assemblyResolver.Init(config);

            var projectAssemblies = _assemblyResolver.LoadAssemblies(IsProjectAssembly);

            if (projectAssemblies.Count == 0)
            {
                return Array.Empty<ISourceCodeExpressionBuilder>();
            }

            var builders = EnumerateBuilders(projectAssemblies).ToList();

            if (builders.Any())
            {
                return builders;
            }

            _logger.Warning(
                $"Source Code Expression {nameof(ISourceCodeExpressionBuilder)}s: " +
                $"no implementations found in project {config.GetProjectNameWithoutExtension()}");

            return Array.Empty<ISourceCodeExpressionBuilder>();
        }

        private static bool IsProjectAssembly(string assemblyName)
        {
            return
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.NetStandardPolyfills") &&
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.ReadableExpressions") &&
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.BuildableExpressions") &&
                assemblyName.DoesNotEqualIgnoreCase("AgileObjects.AgileMapper") &&
                assemblyName.DoesNotStartWithIgnoreCase("System.") &&
                assemblyName.DoesNotStartWithIgnoreCase("Microsoft.") &&
#if NETFRAMEWORK
                assemblyName.DoesNotStartWithIgnoreCase("EnvDTE") &&
                assemblyName.DoesNotStartWithIgnoreCase("stdole") &&
#endif
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
                    $"Source Code Expression {nameof(ISourceCodeExpressionBuilder)}s: " +
                    $"{builderTypes.Count} found in {assembly.GetName().Name}");

                foreach (var builderType in builderTypes)
                {
                    yield return (ISourceCodeExpressionBuilder)Activator.CreateInstance(builderType);
                }
            }
        }
    }
}