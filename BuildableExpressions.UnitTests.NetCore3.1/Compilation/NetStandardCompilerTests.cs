namespace AgileObjects.BuildableExpressions.UnitTests.Compilation
{
    using BuildableExpressions.Compilation;

    public class NetStandardCompilerTests : CompilerTestsBase
    {
        internal override ICSharpCompiler CreateCompiler()
            => new NetStandardCSharpCompiler();
    }
}
