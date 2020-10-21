namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethodCalls
    {
        [Fact]
        public void ShouldBuildAParameterlessThisInstanceCall()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod(Default(typeof(int)));
                });
            });

            var method = sourceCode.TypeExpressions.First().MethodExpressions.First();

            var methodCall = BuildableExpression.Call(method);

            methodCall.NodeType.ShouldBe(ExpressionType.Call);
            methodCall.Type.ShouldBe(typeof(int));

            methodCall.Object
                .ShouldNotBeNull()
                .ShouldBeOfType<ThisInstanceExpression>()
                .Instance.ShouldBeSameAs(method.DeclaringTypeExpression);

            methodCall.Arguments.ShouldBeEmpty();
        }
    }
}
