namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Reflection;

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
    }
}