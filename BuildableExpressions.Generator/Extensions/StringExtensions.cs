namespace AgileObjects.BuildableExpressions.Generator.Extensions
{
    using static System.StringComparison;

    internal static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string value, string query)
            => value.Equals(query, OrdinalIgnoreCase);

        public static bool DoesNotEqualIgnoreCase(this string value, string query)
            => !value.EqualsIgnoreCase(query);

        public static bool StartsWithIgnoreCase(this string value, string substring)
            => value.StartsWith(substring, OrdinalIgnoreCase);

        public static bool DoesNotStartWithIgnoreCase(this string value, string substring)
            => !value.StartsWithIgnoreCase(substring);
    }
}
