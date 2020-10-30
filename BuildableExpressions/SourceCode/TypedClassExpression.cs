namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;

    internal class TypedClassExpression : ClassExpression
    {
        public TypedClassExpression(SourceCodeExpression sourceCode, Type type)
            : base(sourceCode, type.GetBaseType(), type.GetFriendlyName())
        {
            Type = type;
            IsStatic = type.IsAbstract() && type.IsSealed();

            if (!IsStatic)
            {
                IsAbstract = type.IsAbstract();

                if (!IsAbstract)
                {
                    IsSealed = type.IsSealed();
                }
            }

            if (!type.IsGenericType())
            {
                return;
            }

            foreach (var parameterType in type.GetGenericTypeArguments())
            {
                AddGenericParameter(new TypedOpenGenericArgumentExpression(parameterType));
            }
        }

        public override Type Type { get; }
    }
}