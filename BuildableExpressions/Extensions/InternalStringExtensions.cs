namespace AgileObjects.BuildableExpressions.Extensions
{
    using System;
    using SourceCode.Extensions;
    using static System.StringSplitOptions;

    internal static class InternalStringExtensions
    {
        private static readonly string[] _newLines = { Environment.NewLine };

        public static string[] SplitToLines(this string value)
            => value?.Split(_newLines, None) ?? Enumerable<string>.EmptyArray;
    }
}
