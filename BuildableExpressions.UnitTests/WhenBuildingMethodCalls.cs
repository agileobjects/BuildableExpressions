namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using BuildableExpressions.SourceCode.Extensions;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethodCalls
    {
        [Fact]
        public void ShouldBuildAParameterlessThisInstanceCallFromAMethodInfo()
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
