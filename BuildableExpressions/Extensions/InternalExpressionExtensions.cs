namespace AgileObjects.BuildableExpressions.Extensions
{
    using System.Linq.Expressions;
    using ReadableExpressions.Translations.Reflection;
    using SourceCode;

    internal static class InternalExpressionExtensions
    {
        public static bool HasReturnType(this Expression expression)
            => expression.Type != typeof(void);

        public static bool HasReturnType(this MemberExpressionBase member)
            => ((Expression)member).HasReturnType();

        public static bool HasReturnType(this IMember member)
            => !member.Type.Equals(ClrTypeWrapper.Void);
    }
}
