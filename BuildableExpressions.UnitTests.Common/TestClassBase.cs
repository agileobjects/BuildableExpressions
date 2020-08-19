namespace AgileObjects.BuildableExpressions.UnitTests.Common
{
    using System;
    using System.Linq.Expressions;

    public abstract class TestClassBase
    {
        protected static LambdaExpression CreateLambda(Expression<Action> lambda) => lambda;

        protected static LambdaExpression CreateLambda<TArg>(Expression<Action<TArg>> lambda) => lambda;

        protected static LambdaExpression CreateLambda<TArg1, TArg2>(Expression<Action<TArg1, TArg2>> lambda) => lambda;

        public static LambdaExpression CreateLambda<TReturn>(Expression<Func<TReturn>> lambda) => lambda;

        protected static LambdaExpression CreateLambda<TArg, TReturn>(Expression<Func<TArg, TReturn>> lambda) => lambda;

        protected static LambdaExpression CreateLambda<TArg1, TArg2, TReturn>(Expression<Func<TArg1, TArg2, TReturn>> lambda) => lambda;

        protected static LambdaExpression CreateLambda<TArg1, TArg2, TArg3, TReturn>(Expression<Func<TArg1, TArg2, TArg3, TReturn>> lambda) => lambda;
    }
}