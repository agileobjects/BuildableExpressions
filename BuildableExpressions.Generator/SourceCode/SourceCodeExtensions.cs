namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AgileObjects.BuildableExpressions.SourceCode;
    using AgileObjects.ReadableExpressions.Extensions;

    internal static class SourceCodeExtensions
    {
        public static ICollection<SourceCodeExpression> ToSourceCodeExpressions(
            this IEnumerable<ISourceCodeExpressionBuilder> builders,
            IExpressionBuildContext context)
        {
            BuildableExpression.DefaultNamespace ??= context.RootNamespace;

            var allExpressions = new List<SourceCodeExpression>();

            foreach (var builder in builders)
            {
                var expressions = builder.Build(context)?.ToList();

                if (expressions == null)
                {
                    throw NullRefError("null", builder);
                }

                if (expressions.Any(exp => exp == null))
                {
                    throw NullRefError("a null Expression", builder);
                }

                allExpressions.AddRange(expressions);
            }

            return allExpressions;
        }

        private static Exception NullRefError(string description, ISourceCodeExpressionBuilder builder)
        {
            return new InvalidOperationException(
                $"{builder.GetType().GetFriendlyName()}.Build() returned {description}");
        }
    }
}
