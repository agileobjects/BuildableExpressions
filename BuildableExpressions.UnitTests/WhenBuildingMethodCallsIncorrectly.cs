namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using Common;
    using SourceCode;
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

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        var method = cls.AddMethod("GetString", getStringLambda);

                        BuildableExpression.Call(method);
                    });
                });

            });

            argsEx.Message.ShouldContain("Expected 1");
            argsEx.Message.ShouldContain("got 0");
        }

        [Fact]
        public void ShouldErrorIfTooManyParameters()
        {
            var argsEx = Assert.Throws<ArgumentException>(() =>
            {
                var sourceCode = BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod(Default(typeof(int)));
                    });
                });

                var method = sourceCode.Types.First().Methods.First();

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

                var sourceCode = BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("GetLong", getLongLambda);
                    });
                });

                var method = sourceCode.Types.First().Methods.First();

                BuildableExpression.Call(method, Parameter(typeof(string), "str"));
            });

            argsEx.Message.ShouldContain("Parameter 1");
            argsEx.Message.ShouldContain("an Expression of Type 'long'");
            argsEx.Message.ShouldContain("argument is of Type 'string'");
        }
    }
}