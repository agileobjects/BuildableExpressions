namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using System.Collections.Generic;
    using AgileObjects.BuildableExpressions.SourceCode;
    using AgileObjects.ReadableExpressions.Extensions;

    internal static class SourceCodeExtensions
    {
        public static ICollection<SourceCodeExpression> ToSourceCodeExpressions(
            this IEnumerable<ISourceCodeExpressionBuilder> builders)
        {
            var allExpressions = new List<SourceCodeExpression>();

            foreach (var builder in builders)
            {
                var expressions = builder.Build();

                if (expressions == null)
                {
                    throw new InvalidOperationException(
                        $"{builder.GetType().GetFriendlyName()}.Build() returned null");
                }

                allExpressions.AddRange(expressions);
            }

            return allExpressions;
        }
    }
}
