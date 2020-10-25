namespace AgileObjects.BuildableExpressions.SourceCode
{
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal static class MethodExpressionExtensions
    {
        public static string GetSignature(this IMethod method, bool includeTypeName = true)
        {
            var typeName = includeTypeName
                ? GetDeclaringTypeName(method) + "."
                : null;

            var parameterTypeNames = string.Join(", ",
                method.GetParameters().Project(p => p.Type.GetFriendlyName() + " " + p.Name));

            var returnType = method.ReturnType.GetFriendlyName();

            return $"{returnType} {typeName + method.Name}({parameterTypeNames})";
        }

        private static string GetDeclaringTypeName(IMethod method)
        {
            if (method is MethodExpression methodExpression)
            {
                return methodExpression.DeclaringTypeExpression.Name;
            }

            return method.DeclaringType?.GetFriendlyName();
        }
    }
}
