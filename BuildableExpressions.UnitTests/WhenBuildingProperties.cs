namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static BuildableExpressions.SourceCode.MemberVisibility;

    public class WhenBuildingProperties
    {
        [Fact]
        public void ShouldBuildAClassGetSetAutoProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddProperty<string>("MyProperty");
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string MyProperty { get; set; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassPublicGetPrivateSetAutoProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddProperty("MyProperty", typeof(int), p =>
                        {
                            p.SetAutoProperty(setterVisibility: Private);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int MyProperty { get; private set; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructInternalComputedProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("IntPair", str =>
                    {
                        var intOneProperty = str.AddProperty<int>("IntOne");
                        var intTwoProperty = str.AddProperty<int>("IntTwo");

                        str.AddProperty<int>("GreaterValue", p =>
                        {
                            p.SetGetter(g =>
                            {
                                var intOneAccess = Property(
                                    str.ThisInstanceExpression,
                                    intOneProperty.PropertyInfo);

                                var intTwoAccess = Property(
                                    str.ThisInstanceExpression,
                                    intTwoProperty.PropertyInfo);

                                var intOneGreaterThanIntTwo =
                                    GreaterThan(intOneAccess, intTwoAccess);

                                var getGreatestValue =
                                    Condition(intOneGreaterThanIntTwo, intOneAccess, intTwoAccess);

                                g.SetBody(getGreatestValue);
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct IntPair
    {
        public int IntOne { get; set; }

        public int IntTwo { get; set; }

        public int GreaterValue
        {
            get
            {
                return (this.IntOne > this.IntTwo) ? this.IntOne : this.IntTwo;
            }
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructStaticComputedProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("MyStruct", str =>
                    {
                        str.AddProperty<DateTime>("Doomsday", p =>
                        {
                            p.SetStatic();

                            p.SetGetter(g =>
                            {
                                g.SetBody(Property(null, typeof(DateTime), nameof(DateTime.Now)));
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public struct MyStruct
    {
        public static DateTime Doomsday
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceGetOnlyPropertyAndImplementingClass()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var interfaceProperty = default(PropertyExpression);

                    var @interface = sc.AddInterface("IHasName", itf =>
                    {
                        interfaceProperty = itf.AddProperty<string>("Name", p =>
                        {
                            p.SetGetter();
                        });
                    });

                    sc.AddClass("Person", cls =>
                    {
                        cls.SetImplements(@interface, impl =>
                        {
                            impl.AddProperty(interfaceProperty);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public interface IHasName
    {
        string Name { get; }
    }

    public class Person : IHasName
    {
        public string Name { get; private set; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceGetOnlyPropertyAndImplementingStruct()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var interfaceProperty = default(PropertyExpression);

                    var @interface = sc.AddInterface("IHasName", itf =>
                    {
                        interfaceProperty = itf.AddProperty<string>("Name", p =>
                        {
                            p.SetGetter();
                        });
                    });

                    sc.AddStruct("NameOwner", str =>
                    {
                        var firstNameProperty = str.AddProperty<string>("FirstName", p =>
                        {
                            p.SetGetter();
                        });

                        var lastNameProperty = str.AddProperty<string>("LastName", p =>
                        {
                            p.SetGetter();
                        });

                        str.SetImplements(@interface, impl =>
                        {
                            impl.AddProperty(interfaceProperty, p =>
                            {
                                p.SetGetter(g =>
                                {
                                    var concatNames = Call(
                                        typeof(string).GetPublicStaticMethod(
                                            "Concat",
                                            typeof(string), typeof(string), typeof(string)),
                                        Property(
                                            str.ThisInstanceExpression,
                                            firstNameProperty.PropertyInfo),
                                        Constant(" ", typeof(string)),
                                        Property(
                                            str.ThisInstanceExpression,
                                            lastNameProperty.PropertyInfo));

                                    g.SetBody(concatNames);
                                });
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public interface IHasName
    {
        string Name { get; }
    }

    public struct NameOwner : IHasName
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Name
        {
            get
            {
                return this.FirstName + "" "" + this.LastName;
            }
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAbstractClassAbstractProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("BaseClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddProperty<string>("AbstractProperty", p =>
                        {
                            p.SetVisibility(Protected);
                            p.SetAbstract();
                            p.SetGetter();
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class BaseClass
    {
        protected abstract string AbstractProperty { get; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAVirtualProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddProperty<int>("VirtualProperty", p =>
                        {
                            p.SetVisibility(ProtectedInternal);
                            p.SetVirtual();
                            p.SetGetter();
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class MyClass
    {
        protected internal virtual int VirtualProperty { get; private set; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldOverrideABaseAbstractPropertyExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var overrideMe = default(PropertyExpression);

                    var baseType = sc.AddClass("BaseAbstract", cls =>
                    {
                        cls.SetAbstract();

                        overrideMe = cls.AddProperty<string>("OverrideMe", p =>
                        {
                            p.SetAbstract();
                        });
                    });

                    sc.AddClass("DerivedAbstract", cls =>
                    {
                        cls.SetBaseType(baseType);
                        cls.AddProperty(overrideMe);
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class BaseAbstract
    {
        public abstract string OverrideMe { get; set; }
    }

    public class DerivedAbstract : BaseAbstract
    {
        public override string OverrideMe { get; set; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldOverrideABaseVirtualPropertyExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var overrideMe = default(PropertyExpression);

                    var baseType = sc.AddClass("BaseVirtual", cls =>
                    {
                        overrideMe = cls.AddProperty<string>("OverrideMe", p =>
                        {
                            p.SetVirtual();
                        });
                    });

                    sc.AddClass("DerivedVirtual", cls =>
                    {
                        cls.SetBaseType(baseType);
                        cls.AddProperty(overrideMe);
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class BaseVirtual
    {
        public virtual string OverrideMe { get; set; }
    }

    public class DerivedVirtual : BaseVirtual
    {
        public override string OverrideMe { get; set; }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}
