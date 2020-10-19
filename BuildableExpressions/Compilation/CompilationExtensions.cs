namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Logging;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using SourceCode;
    using SourceCode.Extensions;

    internal static class CompilationExtensions
    {
        public static CompilationResult Compile(
            this ICompiler compiler,
            IEnumerable<string> sourceCodes)
        {
            return compiler.Compile(sourceCodes.ToArray());
        }

        public static CompilationResult Compile(
            this ICompiler compiler,
            params string[] sourceCodes)
        {
            return compiler.Compile(Enumerable<Assembly>.EmptyArray, sourceCodes);
        }

        public static CompilationResult Compile(
            this ICompiler compiler,
            params SourceCodeExpression[] sourceCodes)
        {
            return compiler.Compile(
                sourceCodes.SelectMany(sc => sc.ReferencedAssemblies).Distinct(),
                sourceCodes.Project(sc => sc.ToSourceCode()));
        }

        public static CompilationResult Compile(
            this ICompiler compiler,
            IEnumerable<Assembly> referenceAssemblies,
            IEnumerable<string> sourceCodes)
        {
            return compiler.Compile(referenceAssemblies, sourceCodes.ToArray());
        }

        public static ICollection<Assembly> GetReferenceAssemblies(
            this string expressionBuilderSource)
        {
            var referenceAssemblies = new List<Assembly>
            {
                typeof(object).GetAssembly(),
                typeof(AssemblyExtensionsPolyfill).GetAssembly(),
                typeof(ReadableExpression).GetAssembly(),
                typeof(BuildableExpression).GetAssembly()
            };

            if (expressionBuilderSource.Contains("using System.Linq"))
            {
                referenceAssemblies.Add(typeof(Enumerable).GetAssembly());
            }

            return referenceAssemblies;
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
