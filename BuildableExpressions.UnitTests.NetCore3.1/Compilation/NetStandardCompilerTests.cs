namespace AgileObjects.BuildableExpressions.UnitTests.Compilation
{
    using BuildableExpressions.Compilation;

    public class NetStandardCompilerTests : CompilerTestsBase
    {
        internal override ICompiler CreateCompiler()
            => new NetStandardCompiler();
    }
}
