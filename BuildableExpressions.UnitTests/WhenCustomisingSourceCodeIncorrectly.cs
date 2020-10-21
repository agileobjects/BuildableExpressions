namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using SourceCode;
    using Xunit;

    public class WhenCustomisingSourceCodeIncorrectly
    {
        [Fact]
        public void ShouldErrorIfClassMarkedStaticAndAbstract()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetStatic();
                        cls.SetAbstract();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both static and abstract");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedAbstractAndStatic()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetAbstract();
                        cls.SetStatic();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both abstract and static");
        }
    }
}