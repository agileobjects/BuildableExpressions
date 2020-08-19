namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetStandardPolyfills;
    using ReadableExpressions;

    internal static class CompilationExtensions
    {
        public static ICollection<Type> GetReferenceAssemblyTypes(
            this string expressionBuilderSource)
        {
            var referenceAssemblyTypes = new List<Type>
            {
                typeof(object),
                typeof(AssemblyExtensionsPolyfill),
                typeof(ReadableExpression),
                typeof(SourceCodeFactory)
            };

            if (expressionBuilderSource.Contains("using System.Linq"))
            {
                referenceAssemblyTypes.Add(typeof(Enumerable));
            }

            return referenceAssemblyTypes;
        }
    }
}
