namespace AgileObjects.BuildableExpressions.Compilation
{
    internal interface ICompiler
    {
        CompilationResult Compile(string expressionBuilderSource);
    }
}
