namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ReadableExpressions.Extensions;
    using SourceCode;

    /// <summary>
    /// Provides C# code compilation extension methods.
    /// </summary>
    public static class CompilationExtensions
    {
        /// <summary>
        /// Compiles the given <paramref name="sourceCodeExpression"/>, returning an array containing
        /// the compiled CLR Types, or throwing an InvalidOperationException is compilation fails.
        /// </summary>
        /// <param name="compiler">The <see cref="CSharpCompiler"/> with which to perform the compilation.</param>
        /// <param name="sourceCodeExpression">The <see cref="SourceCodeExpression"/> to compile.</param>
        /// <param name="referenceAssemblies">
        /// Any extra reference Assemblies required to compile this
        /// <paramref name="sourceCodeExpression"/>.
        /// </param>
        /// <returns>An array containing the compiled CLR Types.</returns>
        public static Type[] CompileToTypesOrThrow(
            this CSharpCompiler compiler,
            SourceCodeExpression sourceCodeExpression,
            params Assembly[] referenceAssemblies)
        {
            var compilationResult = compiler.Compile(sourceCodeExpression, referenceAssemblies);

            if (!compilationResult.Failed)
            {
                return compilationResult.CompiledAssembly.GetTypes();
            }

            var typeNames = string.Join(", ", sourceCodeExpression
                .TypeExpressions
                .Select(te => te.GetFriendlyName()));

            throw new InvalidOperationException(
                $"Compilation of type(s) '{typeNames}' failed:{Environment.NewLine}" +
                string.Join(Environment.NewLine, compilationResult.Errors));
        }

        /// <summary>
        /// Compiles the given <paramref name="sourceCodeExpression"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="compiler">The <see cref="CSharpCompiler"/> with which to perform the compilation.</param>
        /// <param name="sourceCodeExpression">The <see cref="SourceCodeExpression"/> to compile.</param>
        /// <param name="referenceAssemblies">
        /// Any extra reference Assemblies required to compile this
        /// <paramref name="sourceCodeExpression"/>.
        /// </param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            this CSharpCompiler compiler,
            SourceCodeExpression sourceCodeExpression,
            params Assembly[] referenceAssemblies)
        {
            var allReferenceAssemblies = referenceAssemblies
                .Concat(sourceCodeExpression.ReferencedAssemblies)
                .Distinct();

            return compiler.Compile(
                allReferenceAssemblies,
                sourceCodeExpression.ToSourceCodeString());
        }
    }
}
