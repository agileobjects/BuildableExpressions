namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AgileObjects.BuildableExpressions.SourceCode;
    using NetStandardPolyfills;
    using AgileObjects.ReadableExpressions.Extensions;
    using BuildableExpressions.Compilation;

    internal static class SourceCodeExtensions
    {
        public static IEnumerable<SourceCodeExpression> ToSourceCodeExpressions(
            this CompilationResult compilationResult)
        {
            var allExpressions = new List<SourceCodeExpression>();

            foreach (var builder in GetBuildersOrThrow(compilationResult))
            {
                var expressions = builder.Build();

                if (expressions == null)
                {
                    throw new InvalidOperationException(
                        $"{builder.GetType().GetFriendlyName()}.Build() returned null");
                }

                allExpressions.AddRange(expressions);
            }

            return allExpressions;
        }

        private static IEnumerable<ISourceCodeExpressionBuilder> GetBuildersOrThrow(
            CompilationResult compilationResult)
        {
            var builders = compilationResult
                .CompiledAssembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ISourceCodeExpressionBuilder)))
                .Select(Activator.CreateInstance)
                .Cast<ISourceCodeExpressionBuilder>()
                .ToList();

            if (builders.Count == 0)
            {
                throw new NotSupportedException(
                    $"No {nameof(ISourceCodeExpressionBuilder)} implementations found in project");
            }

            return builders;
        }
    }
}
