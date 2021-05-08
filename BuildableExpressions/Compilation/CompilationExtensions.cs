namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using SourceCode;

    /// <summary>
    /// Provides C# code compilation extension methods.
    /// </summary>
    public static class CompilationExtensions
    {
        /// <summary>
        /// Compiles this <paramref name="sourceCodeExpression"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="sourceCodeExpression">The <see cref="SourceCodeExpression"/>s to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this SourceCodeExpression sourceCodeExpression)
        {
            return CSharpCompiler.Compile(
                sourceCodeExpression.ReferencedAssemblies.Distinct(),
                sourceCodeExpression.ToCSharpString());
        }

        /// <summary>
        /// Compiles these <paramref name="sourceCodeExpressions"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="sourceCodeExpressions">One or more <see cref="SourceCodeExpression"/>s to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this IEnumerable<SourceCodeExpression> sourceCodeExpressions)
        {
            var expressionsList = sourceCodeExpressions.ToList();

            return CSharpCompiler.Compile(
                expressionsList.SelectMany(sc => sc.ReferencedAssemblies).Distinct(),
                expressionsList.Project(sc => sc.ToCSharpString()));
        }
    }
}
