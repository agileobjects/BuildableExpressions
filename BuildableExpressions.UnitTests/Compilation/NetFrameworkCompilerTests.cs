namespace AgileObjects.BuildableExpressions.UnitTests.Compilation
{
    using BuildableExpressions.Compilation;

    public class NetFrameworkCompilerTests : CompilerTestsBase
    {
        internal override ICSharpCompiler CreateCompiler()
            => new NetFrameworkCSharpCompiler();
    }
}
