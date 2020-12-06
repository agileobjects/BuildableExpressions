namespace AgileObjects.BuildableExpressions.UnitTests
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

        public static StructExpression AddStruct(
            this ISourceCodeExpressionConfigurator sourceCodeConfig,
            Action<IStructExpressionConfigurator> configuration)
        {
            return sourceCodeConfig.AddStruct("GeneratedExpressionStruct", configuration);
        }

        public static MethodExpression AddMethod(
            this IClassExpressionConfigurator classConfig,
            string name)
        {
            return classConfig.AddMethod(name, Empty());
        }

        public static MethodExpression AddMethod(
            this IStructExpressionConfigurator structConfig,
            string name)
        {
            return structConfig.AddMethod(name, Empty());
        }

        public static MethodExpression AddMethod(
            this IClassExpressionConfigurator classConfig,
            LambdaExpression defaultVoidLambda)
        {
            return classConfig.AddMethod(defaultVoidLambda.Body);
        }

        public static MethodExpression AddMethod(
            this IStructExpressionConfigurator structConfig,
            LambdaExpression defaultVoidLambda)
        {
            return structConfig.AddMethod(defaultVoidLambda.Body);
        }

        public static MethodExpression AddMethod(
            this IClassExpressionConfigurator classConfig,
            Expression defaultVoid)
        {
            return classConfig.AddMethod("DoAction", defaultVoid);
        }

        public static MethodExpression AddMethod(
            this IStructExpressionConfigurator structConfig,
            Expression defaultVoid)
        {
            return structConfig.AddMethod("DoAction", defaultVoid);
        }
    }
}
