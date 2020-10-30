namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;

    internal class TypedClassExpression : ClassExpression
    {
        public TypedClassExpression(SourceCodeExpression sourceCode, Type type)
            : base(sourceCode, type.GetBaseType(), GetName(type))
        {
            Type = type;

            var isAbstract = type.IsAbstract();
            IsStatic = isAbstract && type.IsSealed();
            IsAbstract = isAbstract && !IsStatic;

            foreach (var parameterType in type.GetGenericTypeArguments())
            {
                AddGenericParameter(new TypedOpenGenericArgumentExpression(parameterType));
            }
        }

        private static string GetName(Type type)
        {
            var name = type.GetFriendlyName();

            return type.IsNested
                ? name.Substring(name.LastIndexOf('.') + 1)
                : name;
        }

        public override Type Type { get; }
    }
}