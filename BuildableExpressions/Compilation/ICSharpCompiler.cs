namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Implementing classes will compile source code into a <see cref="CompilationResult"/>.
    /// </summary>
    public interface ICSharpCompiler
    {
        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/> using the given
        /// <paramref name="referenceAssemblies"/>, returning a <see cref="CompilationResult"/>
        /// describing the results.
        /// </summary>
        /// <param name="referenceAssemblies">
        /// Zero or more Assemblies required for the compilation. The Assemblies in the
        /// <see cref="CSharpCompiler.CompilationAssemblies"/> collection are automatically included
        /// in compilation and do not need to be passed. By default, this includes the Assemblies
        /// defining System.Object, System.Collections.Generic.List{T} and System.Linq.Enumerable.
        /// </param>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        CompilationResult Compile(
            IEnumerable<Assembly> referenceAssemblies,
            params string[] cSharpSourceCodes);
    }
}
