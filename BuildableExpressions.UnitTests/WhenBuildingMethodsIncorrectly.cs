namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethodsIncorrectly
    {
        [Fact]
        public void ShouldErrorIfAbstractMethodAddedToNonAbstractClass()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("ConcreteClass", cls =>
                    {
                        cls.AddMethod("AbstractMethod", m =>
                        {
                            m.SetAbstract();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("Unable to add abstract method");
            methodEx.Message.ShouldContain("'void AbstractMethod()'");
            methodEx.Message.ShouldContain("non-abstract declaring type 'ConcreteClass'");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothAbstractAndStatic()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(string), "str"));
                            m.SetAbstract();
                            m.SetStatic();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(string str)'");
            methodEx.Message.ShouldContain("both abstract and static");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothStaticAndAbstract()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(DateTime), "date"));
                            m.AddParameter(Parameter(typeof(long), "number"));
                            m.SetStatic();
                            m.SetAbstract();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(DateTime date, long number)'");
            methodEx.Message.ShouldContain("both static and abstract");
        }
    }
}