namespace AgileObjects.BuildableExpressions.Extensions
{
    using System;

    internal static class InternalValidationExtensions
    {
        public static void ThrowIfNull<T>(this T obj, string paramName)
            where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(
                    $"Parameter '{paramName}' cannot be null",
                    paramName);
            }
        }
    }
}
