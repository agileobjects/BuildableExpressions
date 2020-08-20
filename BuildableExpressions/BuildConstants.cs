namespace AgileObjects.BuildableExpressions
{
    internal static class BuildConstants
    {
        public const string InputFileKey = "BuildableExpressionsInput";
        public const string OutputDirectoryKey = "BuildableExpressionsOutput";

        public const string InputClass = "ExpressionBuilder";
        public const string InputMethod = "Build";

        public const string DefaultInputFile = InputClass + ".cs";
        public const string DefaultOutputFile = InputClass + "Output.cs";
    }
}
