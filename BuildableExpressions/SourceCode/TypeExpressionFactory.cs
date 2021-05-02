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

        public static ClassExpression CreateClass(Type type) 
            => (ClassExpression)_cache.GetOrAdd(type, t => new TypedClassExpression(t));

        public static TypeExpression Create(Type type) 
            => _cache.GetOrAdd(type, CreateTypeExpression);

        private static TypeExpression CreateTypeExpression(Type type)
        {
            if (type.IsClass())
            {
                return new TypedClassExpression(type);
            }

            if (type.IsInterface())
            {
                return new TypedInterfaceExpression(type);
            }

            if (type.IsEnum())
            {
                return new TypedEnumExpression(type);
            }

            if (type.IsGenericParameter)
            {
                return new TypedGenericParameterExpression(type);
            }

            return new TypedStructExpression(type);
        }
    }
}