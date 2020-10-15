namespace AgileObjects.BuildableExpressions.Compilation
{
    internal static class Compiler
    {
        public static readonly ICompiler Instance =
#if NETFRAMEWORK
            new NetFrameworkCompiler();
#else
            new NetStandardCompiler();
#endif
    }
}