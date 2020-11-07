namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Generics;
    using NetStandardPolyfills;

    internal class TypedInterfaceExpression : InterfaceExpression
    {
        public TypedInterfaceExpression(SourceCodeExpression sourceCode, Type @interface)
            : base(sourceCode, @interface.GetTypedExpressionName())
        {
            Type = @interface;

            foreach (var parameterType in @interface.GetGenericTypeArguments())
            {
                AddGenericParameter(new TypedOpenGenericArgumentExpression(parameterType));
            }
        }

        public override Type Type { get; }
    }
}