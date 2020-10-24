namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Logging;
    using SourceCode;
    using SourceCode.Extensions;

    /// <summary>
    /// Provides C# code compilation extension methods.
    /// </summary>
    public static class CompilationExtensions
    {
        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="compiler">The <see cref="ICSharpCompiler"/> to use.</param>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            IEnumerable<string> cSharpSourceCodes)
        {
            return compiler.Compile(cSharpSourceCodes.ToArray());
        }

        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="compiler">The <see cref="ICSharpCompiler"/> to use.</param>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            params string[] cSharpSourceCodes)
        {
            return compiler.Compile(Enumerable<Assembly>.EmptyArray, cSharpSourceCodes);
        }

        /// <summary>
        /// Compiles the given <paramref name="sourceCodeExpressions"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="compiler">The <see cref="ICSharpCompiler"/> to use.</param>
        /// <param name="sourceCodeExpressions">One or more <see cref="SourceCodeExpression"/>s to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            params SourceCodeExpression[] sourceCodeExpressions)
        {
            return compiler.Compile(
                sourceCodeExpressions.SelectMany(sc => sc.ReferencedAssemblies).Distinct(),
                sourceCodeExpressions.Project(sc => sc.ToCSharpString()));
        }

        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/> using the given
        /// <paramref name="referenceAssemblies"/>, returning a <see cref="CompilationResult"/>
        /// describing the results.
        /// </summary>
        /// <param name="compiler">The <see cref="ICSharpCompiler"/> to use.</param>
        /// <param name="referenceAssemblies">
        /// Zero or more Assemblies required for the compilation. The Assemblies in the
        /// <see cref="CSharpCompiler.CompilationAssemblies"/> collection are automatically included
        /// in compilation and do not need to be passed. By default, this includes the Assemblies
        /// defining System.Object, System.Collections.Generic.List{T} and System.Linq.Enumerable.
        /// </param>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this ICSharpCompiler compiler,
            IEnumerable<Assembly> referenceAssemblies,
            IEnumerable<string> cSharpSourceCodes)
        {
            return compiler.Compile(referenceAssemblies, cSharpSourceCodes.ToArray());
        }

        internal static bool CompilationFailed(
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
