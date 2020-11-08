namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using Xunit;

    public class WhenBuildingPropertiesIncorrectly
    {
        [Fact]
        public void ShouldErrorIfAbstractPropertyAddedToNonAbstractClass()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("ConcreteClass", cls =>
                    {
                        cls.AddProperty<int>("AbstractInt", m =>
                        {
                            m.SetAbstract();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("Unable to add abstract property");
            propertyEx.Message.ShouldContain("'int AbstractInt'");
            propertyEx.Message.ShouldContain("non-abstract declaring type 'ConcreteClass'");
        }

        [Fact]
        public void ShouldErrorIfClassPropertyMarkedBothAbstractAndStatic()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddProperty("MyProperty", typeof(DateTime), p =>
                        {
                            p.SetAbstract();
                            p.SetStatic();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'DateTime MyProperty'");
            propertyEx.Message.ShouldContain("both abstract and static");
        }

        [Fact]
        public void ShouldErrorIfClassPropertyMarkedBothStaticAndAbstract()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddProperty<long>("MyProperty", p =>
                        {
                            p.SetStatic();
                            p.SetAbstract();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'long MyProperty'");
            propertyEx.Message.ShouldContain("both static and abstract");
        }
    }
}