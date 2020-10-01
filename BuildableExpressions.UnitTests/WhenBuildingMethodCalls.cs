namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Linq.Expressions;
    using Common;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethodCalls
    {
        [Fact]
        public void ShouldBuildAParameterlessThisInstanceCall()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var method = sourceCode.AddClass().AddMethod(Default(typeof(int)));

            var methodCall = BuildableExpression.Call(method);

            methodCall.NodeType.ShouldBe(ExpressionType.Call);
            methodCall.Type.ShouldBe(typeof(int));

            methodCall.Object
                .ShouldNotBeNull()
                .ShouldBeOfType<ThisInstanceExpression>()
                .Class.ShouldBeSameAs(method.Class);

            methodCall.Arguments.ShouldBeEmpty();
        }
    }
}
