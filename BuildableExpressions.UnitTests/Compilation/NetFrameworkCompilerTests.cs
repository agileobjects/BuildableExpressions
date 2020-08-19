namespace AgileObjects.BuildableExpressions.UnitTests.Compilation
{
    using BuildableExpressions.Compilation;

    public class NetFrameworkCompilerTests : CompilerTestsBase
    {
        internal override ICompiler CreateCompiler()
            => new NetFrameworkCompiler();
    }
}
