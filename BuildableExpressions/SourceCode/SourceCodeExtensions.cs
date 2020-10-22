namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using static System.Linq.Expressions.Expression;

    internal static class SourceCodeExtensions
    {
        public static LambdaExpression ToLambdaExpression(
            this Expression expression,
            IList<ParameterExpression> parameters,
            Type returnType = null)
        {
            returnType ??= expression.Type;

            var isAction =
                 returnType == typeof(void) ||
                !expression.HasReturnType();

            Type lambdaType;

            if ((parameters?.Count ?? 0) == 0)
            {
                lambdaType = isAction
                    ? GetActionType()
                    : GetFuncType(returnType);

                return Lambda(lambdaType, expression);
            }

            var parameterCount = parameters.Count;

            Type[] lambdaParameterTypes;

            if (isAction)
            {
                lambdaParameterTypes = parameters.ProjectToArray(p => p.Type);
                lambdaType = GetActionType(lambdaParameterTypes);

                return Lambda(lambdaType, expression, parameters);
            }

            lambdaParameterTypes = new Type[parameterCount + 1];

            for (var i = 0; ;)
            {
                lambdaParameterTypes[i] = parameters[i].Type;
                ++i;

                if (i != parameterCount)
                {
                    continue;
                }

                lambdaParameterTypes[parameterCount] = returnType;
                lambdaType = GetFuncType(lambdaParameterTypes);

                return Lambda(lambdaType, expression, parameters);
            }
        }
    }
}