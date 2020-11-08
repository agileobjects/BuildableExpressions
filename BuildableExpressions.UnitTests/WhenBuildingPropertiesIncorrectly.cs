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
        public void ShouldErrorIfClassPropertyMarkedBothAbstractAndVirtual()
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
                            p.SetVirtual();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'DateTime MyProperty'");
            propertyEx.Message.ShouldContain("both abstract and virtual");
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

        [Fact]
        public void ShouldErrorIfClassPropertyMarkedBothStaticAndVirtual()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddProperty<long>("MyProperty", p =>
                        {
                            p.SetStatic();
                            p.SetVirtual();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'long MyProperty'");
            propertyEx.Message.ShouldContain("both static and virtual");
        }

        [Fact]
        public void ShouldErrorIfClassPropertyMarkedBothVirtualAndStatic()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddProperty<long>("MyProperty", p =>
                        {
                            p.SetVirtual();
                            p.SetStatic();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'long MyProperty'");
            propertyEx.Message.ShouldContain("both virtual and static");
        }

        [Fact]
        public void ShouldErrorIfClassPropertyMarkedBothVirtualAndAbstract()
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
                            p.SetVirtual();
                            p.SetAbstract();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'long MyProperty'");
            propertyEx.Message.ShouldContain("both virtual and abstract");
        }

        [Fact]
        public void ShouldErrorIfStructAutoPropertyGivenGetter()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddStruct("MyStruct", str =>
                    {
                        str.AddProperty<int>("Prop", p =>
                        {
                            p.SetAutoProperty();
                            p.SetGetter();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'int Prop'");
            propertyEx.Message.ShouldContain("multiple get accessors configured");
        }

        [Fact]
        public void ShouldErrorIfInterfacePropertyGivenMultipleSetters()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddInterface("INameable", itf =>
                    {
                        itf.AddProperty<string>("Name", p =>
                        {
                            p.SetSetter();
                            p.SetSetter();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'string Name'");
            propertyEx.Message.ShouldContain("multiple set accessors configured");
        }

        [Fact]
        public void ShouldErrorIfClassGetPropertyMadeAutoProperty()
        {
            var propertyEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyTestClass", cls =>
                    {
                        cls.AddProperty<TimeSpan>("Prop", p =>
                        {
                            p.SetGetter();
                            p.SetAutoProperty();
                        });
                    });
                });
            });

            propertyEx.Message.ShouldContain("'TimeSpan Prop'");
            propertyEx.Message.ShouldContain("multiple get accessors configured");
        }
    }
}