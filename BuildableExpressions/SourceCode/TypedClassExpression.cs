namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Generics;
    using NetStandardPolyfills;

    internal class TypedClassExpression : ClassExpression
    {
        public TypedClassExpression(SourceCodeExpression sourceCode, Type type)
            : base(sourceCode, type.GetBaseType(), type.GetTypedExpressionName())
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

        public override Type Type { get; }
    }
}