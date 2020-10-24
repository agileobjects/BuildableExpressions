namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using BuildableExpressions.SourceCode.Extensions;
    using Common;
    using NetStandardPolyfills;
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

        [Fact]
        public void ShouldBuildAParameterlessThisInstanceCallWithAMethodInfo()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var getThisMethod = cls.AddMethod("GetThis", m =>
                    {
                        m.SetBody(cls.ThisInstanceExpression);
                    });

                    var getThisCall = Call(
                        cls.ThisInstanceExpression,
                        getThisMethod.MethodInfo);

                    cls.AddMethod("CallGetThis", m =>
                    {
                        m.SetBody(getThisCall);
                    });
                });
            });

            var classType = sourceCode
                .TypeExpressions
                .FirstOrDefault().ShouldNotBeNull()
                .Type;

            var classInstance = Activator
                .CreateInstance(classType).ShouldNotBeNull();

            var method = classType
                .GetPublicInstanceMethod("CallGetThis").ShouldNotBeNull();

            method.Invoke(classInstance, Enumerable<object>.EmptyArray)
                .ShouldNotBeNull()
                .ShouldBeSameAs(classInstance);
        }
    }
}
