namespace AgileObjects.BuildableExpressions.Generator
{
    using System.Reflection;
    using NetStandardPolyfills;

    internal static class GeneratorConstants
    {
        public static readonly Assembly ThisAssembly = typeof(GeneratorConstants).GetAssembly();
        
        public const string SampleInputFileNamespace = "DefaultNamespace";
        public const string SampleInputFileName = "ExpressionBuilder.cs";
    }
}
