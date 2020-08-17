namespace AgileObjects.BuildableExpressions.Extensions
{
    using System.Linq.Expressions;

    internal static class InternalExpressionExtensions
    {
        public static bool HasReturnType(this Expression expression)
            => expression.Type != typeof(void);
    }
}
