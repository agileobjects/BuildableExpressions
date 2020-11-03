namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
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
            EXPECTED.ShouldCompile();
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
            EXPECTED.ShouldCompile();
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
            EXPECTED.ShouldCompile();
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
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}
