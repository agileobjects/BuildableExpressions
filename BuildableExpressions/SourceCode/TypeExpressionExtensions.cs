namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Linq;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal static class TypeExpressionExtensions
    {
        public static string GetMethodName(
            this TypeExpression typeExpression,
            MethodExpression method)
        {
            var typeMethods = typeExpression.MethodExpressionsByReturnType[method.ReturnType];

            if (typeMethods.Count == 1)
            {
                return method.GetDefaultName();
            }

            if (method.Parameters.Count == 0)
            {
                goto AddSuffix;
            }

            var methodParameterTypes = method
                .Parameters
                .ProjectToArray(p => p.Type);

            var matchingMethodExists = typeMethods.Any(m =>
                m != method &&
                m.Parameters.Select(p => p.Type).SequenceEqual(methodParameterTypes));

            if (!matchingMethodExists)
            {
                return method.GetDefaultName();
            }

        AddSuffix:
            var methodIndex = typeMethods.IndexOf(method);

            return method.GetDefaultName() + (methodIndex + 1);
        }

        private static string GetDefaultName(this IMethod method)
        {
            return (method.ReturnType != typeof(void))
                ? "Get" + method.ReturnType.GetVariableNameInPascalCase()
                : "DoAction";
        }
    }
}