namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NetStandardPolyfills;
    using SourceCode;
    using static BuildConstants;

    internal class CompilationResult
    {
        public bool Failed => CompiledAssembly == null;

        public Assembly CompiledAssembly { get; set; }

        public ICollection<string> Errors { get; set; }

        public IEnumerable<SourceCodeExpression> ToSourceCodeExpressions()
        {
            var builderType = GetBuilderTypeOrThrow();
            var buildMethod = GetBuildMethodOrThrow(builderType);
            var buildMethodResult = buildMethod.Invoke(null, Array.Empty<object>());

            if (buildMethodResult == null)
            {
                throw new InvalidOperationException($"{InputClass}.{InputMethod} returned null");
            }

            return (IEnumerable<SourceCodeExpression>)buildMethodResult;
        }

        private Type GetBuilderTypeOrThrow()
        {
            var builderTypes = CompiledAssembly
                .GetTypes()
                .Where(t => t.Name == InputClass)
                .ToList();

            if (builderTypes.Count == 0)
            {
                throw new NotSupportedException($"Expected input Type {InputClass} not found");
            }

            if (builderTypes.Count > 1)
            {
                throw new NotSupportedException($"Multiple {InputClass} Types found");
            }

            return builderTypes[0];
        }

        private static MethodInfo GetBuildMethodOrThrow(Type builderType)
        {
            var buildMethod = builderType.GetPublicStaticMethod(InputMethod);

            if (buildMethod == null)
            {
                throw new NotSupportedException(
                    $"Expected public, static method {InputClass}.{InputMethod} not found");
            }

            if (buildMethod.GetParameters().Any())
            {
                throw new NotSupportedException(
                    $"Expected method {InputClass}.{InputMethod} to be parameterless");
            }

            if (!buildMethod.ReturnType.IsAssignableTo(typeof(IEnumerable<SourceCodeExpression>)))
            {
                throw new NotSupportedException(
                    $"Expected method {InputClass}.{InputMethod} to return " +
                    $"IEnumerable<{nameof(SourceCodeExpression)}>");
            }

            return buildMethod;
        }
    }
}