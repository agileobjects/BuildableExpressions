namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using SourceCode;

    internal class CompilationResult
    {
        public bool Failed => CompiledAssembly == null;

        public Assembly CompiledAssembly { get; set; }

        public ICollection<string> Errors { get; set; }

        public IEnumerable<SourceCodeExpression> ToSourceCodeExpressions()
        {
            var allExpressions = new List<SourceCodeExpression>();

            foreach (var builder in GetBuildersOrThrow())
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

        private IEnumerable<ISourceCodeExpressionBuilder> GetBuildersOrThrow()
        {
            var builders = CompiledAssembly
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