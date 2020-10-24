namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using SourceCode;

    /// <summary>
    /// Contains the results of a code-compilation operation.
    /// </summary>
    public class CompilationResult
    {
        /// <summary>
        /// Gets a value indicating whether the code compilation failed.
        /// </summary>
        public bool Failed => CompiledAssembly == null;

        /// <summary>
        /// Gets the Assembly which contains the result of the compilation.
        /// </summary>
        public Assembly CompiledAssembly { get; internal set; }

        /// <summary>
        /// Gets descriptions of any errors which occurred during compilation.
        /// </summary>
        public IEnumerable<string> Errors { get; internal set; }

        /// <summary>
        /// Retrieves <see cref="SourceCodeExpression"/>s built by the
        /// <see cref="ISourceCodeExpressionBuilder"/>s in the <see cref="CompiledAssembly"/>.
        /// </summary>
        /// <returns>
        /// <see cref="SourceCodeExpression"/>s built by the <see cref="ISourceCodeExpressionBuilder"/>s
        /// in the <see cref="CompiledAssembly"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the <see cref="CompiledAssembly"/> does not contain any
        /// <see cref="ISourceCodeExpressionBuilder"/> implementations.
        /// </exception>
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