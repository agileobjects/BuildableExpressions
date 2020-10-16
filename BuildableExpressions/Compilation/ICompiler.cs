namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Reflection;

    internal interface ICompiler
    {
        CompilationResult Compile(
            IEnumerable<Assembly> referenceAssemblies,
            params string[] sourceCodes);
    }
}
