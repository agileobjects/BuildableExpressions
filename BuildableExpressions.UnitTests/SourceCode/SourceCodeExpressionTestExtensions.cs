namespace AgileObjects.BuildableExpressions.UnitTests.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using BuildableExpressions.SourceCode.Api;
    using static System.Linq.Expressions.Expression;

    internal static class SourceCodeExpressionTestExtensions
    {
        public static ClassExpression AddClass(
            this ISourceCodeExpressionConfigurator sourceCodeConfig,
            Action<IClassExpressionConfigurator> configuration)
        {
            return sourceCodeConfig.AddClass("GeneratedExpressionClass", configuration);
        }

        public static MethodExpression AddMethod(
            this ITypeExpressionConfigurator classConfig,
            string name)
        {
            return classConfig.AddMethod(name, Default(typeof(void)));
        }

        public static MethodExpression AddMethod(
            this ITypeExpressionConfigurator classConfig,
            LambdaExpression defaultVoidLambda)
        {
            return classConfig.AddMethod(defaultVoidLambda.Body);
        }

        public static MethodExpression AddMethod(
            this ITypeExpressionConfigurator classConfig,
            Expression defaultVoid)
        {
            return classConfig.AddMethod("DoAction", defaultVoid);
        }
    }
}
