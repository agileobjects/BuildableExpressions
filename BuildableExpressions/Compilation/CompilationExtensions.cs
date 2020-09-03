namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using NetStandardPolyfills;
    using ReadableExpressions;

    internal static class CompilationExtensions
    {
        public static ICollection<Type> GetReferenceAssemblyTypes(
            this string expressionBuilderSource)
        {
            var referenceAssemblyTypes = new List<Type>
            {
                typeof(object),
                typeof(AssemblyExtensionsPolyfill),
                typeof(ReadableExpression),
                typeof(SourceCodeFactory)
            };

            if (expressionBuilderSource.Contains("using System.Linq"))
            {
                referenceAssemblyTypes.Add(typeof(Enumerable));
            }

            return referenceAssemblyTypes;
        }

        public static bool CompilationFailed(
            this ICompiler compiler,
            IEnumerable<string> expressionBuilderSources,
            ILogger logger,
            out CompilationResult compilationResult)
        {
            compilationResult = compiler.Compile(expressionBuilderSources);

            if (!compilationResult.Failed)
            {
                return false;
            }

            logger.Error("Expression compilation failed:");

            foreach (var error in compilationResult.Errors)
            {
                logger.Error(error);
            }

            return true;
        }
    }
}
