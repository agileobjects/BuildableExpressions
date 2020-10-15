namespace AgileObjects.BuildableExpressions.SourceCode
{
    using Api;

    internal static class MethodExpressionExtensions
    {
        public static string GetGenericParameterName(
            this MethodExpression methodExpression,
            IGenericParameterNamingContext paramCtx)
        {
            const string PARAM_NAME = "T";

            if (methodExpression.GenericArguments.Count == 1)
            {
                return PARAM_NAME;
            }

            var parameter = (GenericParameterExpression)paramCtx;
            var parameterIndex = methodExpression.GenericArguments.IndexOf(parameter);

            return PARAM_NAME + (parameterIndex + 1);
        }
    }
}