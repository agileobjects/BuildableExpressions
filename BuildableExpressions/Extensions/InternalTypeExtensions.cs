namespace AgileObjects.BuildableExpressions.Extensions
{
    using System;
#if NET_STANDARD
    using System.Reflection;
#endif

    internal static class InternalTypeExtensions
    {
        public static bool IsGenericTypeDefinition(this Type type)
        {
#if NET_STANDARD
            return type.GetTypeInfo().IsGenericTypeDefinition;
#else
            return type.IsGenericTypeDefinition;
#endif
        }
    }
}
