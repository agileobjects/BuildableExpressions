namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethodCallsIncorrectly
    {
        [Fact]
        public void ShouldErrorIfTooFewParameters()
        {
            var argsEx = Assert.Throws<ArgumentException>(() =>
            {
                var stringParam = Parameter(typeof(string), "str");
                var getStringLambda = Lambda<Func<string, string>>(stringParam, stringParam);

                var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
                var method = sourceCode.AddClass().AddMethod(getStringLambda);

                BuildableExpression.Call(method);
            });

            argsEx.Message.ShouldContain("Expected 1");
            argsEx.Message.ShouldContain("got 0");
        }

        [Fact]
        public void ShouldErrorIfTooManyParameters()
        {
            var argsEx = Assert.Throws<ArgumentException>(() =>
            {
                var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
                var method = sourceCode.AddClass().AddMethod(Default(typeof(int)));

                BuildableExpression.Call(
                    method,
                    Parameter(typeof(int), "param1"),
                    Parameter(typeof(int), "param2"));
            });

            argsEx.Message.ShouldContain("Expected 0");
            argsEx.Message.ShouldContain("got 2");
        }

        [Fact]
        public void ShouldErrorIfWrongParameterType()
        {
            var argsEx = Assert.Throws<ArgumentException>(() =>
            {
                var longParam = Parameter(typeof(long), "lng");
                var getLongLambda = Lambda<Func<long, long>>(longParam, longParam);

                var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
                var method = sourceCode.AddClass().AddMethod(getLongLambda);

                BuildableExpression.Call(method, Parameter(typeof(string), "str"));
            });

            argsEx.Message.ShouldContain("Parameter 1");
            argsEx.Message.ShouldContain("an Expression of Type 'long'");
            argsEx.Message.ShouldContain("argument is of Type 'string'");
        }
    }
}