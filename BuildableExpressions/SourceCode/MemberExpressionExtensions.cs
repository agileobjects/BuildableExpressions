namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Collections.Generic;
    using System.Linq;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal static class MemberExpressionExtensions
    {
        public static string GetMethodSignature(this IMethod method, bool includeTypeName = true)
        {
            var typeName = includeTypeName
                ? GetDeclaringTypeName(method) + "."
                : null;

            var parameterTypeNames = string.Join(", ",
                method.GetParameters().Project(p => p.Type.GetFriendlyName() + " " + p.Name));

            var returnType = method.ReturnType.GetFriendlyName();

            return $"{returnType} {typeName + method.Name}({parameterTypeNames})";
        }

        private static string GetDeclaringTypeName(IMember method)
        {
            if (method is MethodExpression methodExpression)
            {
                return methodExpression.DeclaringTypeExpression.Name;
            }

            return method.DeclaringType?.GetFriendlyName();
        }

        public static IEnumerable<TMemberExpression> GetAllVirtualMembersOfType<TMemberExpression>(
            this TypeExpression typeExpression)
            where TMemberExpression : IConcreteTypeExpression
        {
            if (typeExpression is ClassExpression classExpression)
            {
                return classExpression.BaseTypeExpression
                    .GetAllVirtualMembersOfType<TMemberExpression>();
            }

            return Enumerable<TMemberExpression>.EmptyArray;
        }

        private static IEnumerable<TMemberExpression> GetAllVirtualMembersOfType<TMemberExpression>(
            this ClassExpression classExpression)
            where TMemberExpression : IConcreteTypeExpression
        {
            if (classExpression?.MemberExpressionsAccessor == null)
            {
                yield break;
            }

            var candidateMembers = classExpression
                .MemberExpressionsAccessor
                .OfType<TMemberExpression>()
                .Filter(m => m.IsVirtual)
                .Concat(classExpression.BaseTypeExpression
                    .GetAllVirtualMembersOfType<TMemberExpression>());

            foreach (var candidateMember in candidateMembers)
            {
                yield return candidateMember;
            }
        }
    }
}
