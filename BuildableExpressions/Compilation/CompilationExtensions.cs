namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Logging;
    using SourceCode;
    using SourceCode.Extensions;

    internal static class CompilationExtensions
    {
        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            IEnumerable<string> sourceCodes)
        {
            return compiler.Compile(sourceCodes.ToArray());
        }

        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            params string[] sourceCodes)
        {
            return compiler.Compile(Enumerable<Assembly>.EmptyArray, sourceCodes);
        }

        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            params SourceCodeExpression[] sourceCodes)
        {
            return compiler.Compile(
                sourceCodes.SelectMany(sc => sc.ReferencedAssemblies).Distinct(),
                sourceCodes.Project(sc => sc.ToCSharpString()));
        }

        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            IEnumerable<Assembly> referenceAssemblies,
            IEnumerable<string> sourceCodes)
        {
            return compiler.Compile(referenceAssemblies, sourceCodes.ToArray());
        }

        public static bool CompilationFailed(
            this ICSharpCompiler compiler,
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
