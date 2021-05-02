namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Concurrent;
    using Generics;
    using NetStandardPolyfills;

    internal static class TypeExpressionFactory
    {
        private static readonly ConcurrentDictionary<Type, TypeExpression> _cache = new ();

        public static ClassExpression CreateClass(Type type)
            => (ClassExpression)_cache.GetOrAdd(type, CreateTypedClass);

        public static InterfaceExpression CreateInterface(Type type)
            => (InterfaceExpression)_cache.GetOrAdd(type, CreateTypedInterface);

        public static TypeExpression Create(Type type)
            => _cache.GetOrAdd(type, CreateTypeExpression);

        private static TypeExpression CreateTypeExpression(Type type)
        {
            if (type.IsClass())
            {
                return CreateTypedClass(type);
            }

            if (type.IsInterface())
            {
                return CreateTypedInterface(type);
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

        private static TypedClassExpression CreateTypedClass(Type classType) 
            => new (classType);

        private static TypedInterfaceExpression CreateTypedInterface(Type interfaceType) 
            => new (interfaceType);
    }
}