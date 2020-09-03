namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;

    internal interface ICompiler
    {
        CompilationResult Compile(IEnumerable<string> expressionBuilderSources);
    }
}
