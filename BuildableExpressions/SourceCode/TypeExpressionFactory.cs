namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Concurrent;
    using Generics;
    using NetStandardPolyfills;

    internal static class TypeExpressionFactory
    {
        private static readonly ConcurrentDictionary<Type, TypeExpression> _cache =
            new ConcurrentDictionary<Type, TypeExpression>();

        public static TypeExpression Create(Type type)
        {
            return _cache.GetOrAdd(type, t =>
            {
                if (t.IsInterface())
                {
                    return new TypedInterfaceExpression(t);
                }

                if (t.IsClass())
                {
                    return new TypedClassExpression(t);
                }

                if (t.IsEnum())
                {
                    return new TypedEnumExpression(t);
                }

                if (t.IsGenericParameter)
                {
                    return new TypedGenericParameterExpression(t);
                }

                return new TypedStructExpression(t);
            });
        }
    }
}