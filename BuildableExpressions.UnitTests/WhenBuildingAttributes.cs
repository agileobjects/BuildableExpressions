namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using Xunit;
    using static System.AttributeTargets;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingAttributes
    {
        [Fact]
        public void ShouldBuildAParameterlessClassAttribute()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("SpecialFunkyAttribute", _ => { });

                    sc.AddClass("HasAnAttribute", cls =>
                    {
                        cls.AddAttribute(attribute);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public class SpecialFunkyAttribute : Attribute
    {
    }

    [SpecialFunky]
    public class HasAnAttribute
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructAttributeWithABaseType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("DerivedFunkyAttribute", attr =>
                    {
                        attr.SetBaseType<BaseStructAttribute>();
                    });

                    sc.AddStruct("AttributedStruct", str =>
                    {
                        str.AddAttribute(attribute);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public class DerivedFunkyAttribute : WhenBuildingAttributes.BaseStructAttribute
    {
    }

    [DerivedFunky]
    public struct AttributedStruct
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassAttributeWithABaseAttributeExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var parentAttribute = sc.AddAttribute("ParentAttribute", attr =>
                    {
                        attr.SetAbstract();
                    });

                    var childAttribute = sc.AddAttribute("DerivedAttribute", attr =>
                    {
                        attr.SetBaseType(parentAttribute, _ => { });
                        attr.SetSealed();
                    });

                    sc.AddClass("AttributedClass", str =>
                    {
                        str.AddAttribute(childAttribute);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public abstract class ParentAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class DerivedAttribute : ParentAttribute
    {
    }

    [Derived]
    public class AttributedClass
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAttributeWithAConstructor()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("SecretNameAttribute", attr =>
                    {
                        attr.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("name");
                            ctor.SetBody(Empty());
                        });
                    });

                    sc.AddClass("HasAnAttribute", cls =>
                    {
                        cls.AddAttribute(attribute, attr =>
                        {
                            attr.SetConstructorArguments("It's a secret!");
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public class SecretNameAttribute : Attribute
    {
        public SecretNameAttribute
        (
            string name
        )
        {
        }
    }

    [SecretName(""It's a secret!"")]
    public class HasAnAttribute
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldSelectAttributeConstructorByArguments()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("SecretNamesAttribute", attr =>
                    {
                        var namesProperty = attr.AddProperty<string[]>("Names", p => p.SetGetter());

                        attr.AddConstructor(ctor =>
                        {
                            var nameParameter = ctor.AddParameter<string>("name");

                            ctor.SetBody(Assign(
                                Property(attr.ThisInstanceExpression, namesProperty.PropertyInfo),
                                NewArrayInit(typeof(string), nameParameter)));
                        });

                        attr.AddConstructor(ctor =>
                        {
                            var name1Parameter = ctor.AddParameter<string>("name1");
                            var name2Parameter = ctor.AddParameter<string>("name2");

                            ctor.SetBody(Assign(
                                Property(attr.ThisInstanceExpression, namesProperty.PropertyInfo),
                                NewArrayInit(typeof(string), name1Parameter, name2Parameter)));
                        });
                    });

                    sc.AddClass("HasOneName", cls =>
                    {
                        cls.AddAttribute(attribute, attr =>
                        {
                            attr.SetConstructorArguments("One name");
                        });
                    });

                    sc.AddClass("HasTwoNames", cls =>
                    {
                        cls.AddAttribute(attribute, attr =>
                        {
                            attr.SetConstructorArguments("First name", "Second name");
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public class SecretNamesAttribute : Attribute
    {
        public SecretNamesAttribute
        (
            string name
        )
        {
            this.Names = new[] { name };
        }

        public SecretNamesAttribute
        (
            string name1,
            string name2
        )
        {
            this.Names = new[] { name1, name2 };
        }

        public string[] Names { get; private set; }
    }

    [SecretNames(""One name"")]
    public class HasOneName
    {
    }

    [SecretNames(""First name"", ""Second name"")]
    public class HasTwoNames
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldSupportAnAttributeWithANullConstructorArgument()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("SecretNameAttribute", attr =>
                    {
                        attr.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("name");
                            ctor.SetBody(Empty());
                        });
                    });

                    sc.AddClass("HasNullName", cls =>
                    {
                        cls.AddAttribute(attribute, attr =>
                        {
                            attr.SetConstructorArguments(default(string));
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public class SecretNameAttribute : Attribute
    {
        public SecretNameAttribute
        (
            string name
        )
        {
        }
    }

    [SecretName(null)]
    public class HasNullName
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldSelectAttributeConstructorUsingANullArgument()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("PassesNull", cls =>
                    {
                        cls.AddAttribute<CtorHelperAttribute>(attr =>
                        {
                            attr.SetConstructorArguments(default(string));
                        });

                        cls.SetSealed();
                    });
                })
                .ToCSharpString();

            const string expected = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    [WhenBuildingAttributes.CtorHelper(null)]
    public sealed class PassesNull
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAttributeWithCustomUsage()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("TypesOnlyAttribute", attr =>
                    {
                        attr.SetValidOn(Class | Struct | Interface);
                    });

                    sc.AddClass("HasAnAttribute", cls =>
                    {
                        cls.AddAttribute(attribute);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class TypesOnlyAttribute : Attribute
    {
    }

    [TypesOnly]
    public class HasAnAttribute
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldApplyMultipleAttributes()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute1 = sc.AddAttribute("Attribute1Attribute", _ => { });
                    var attribute2 = sc.AddAttribute("Attribute2Attribute", _ => { });

                    sc.AddClass("HasManyAttributes", cls =>
                    {
                        cls.AddAttribute(attribute1);
                        cls.AddAttribute(attribute2);
                        cls.AddAttribute<TestAttribute>();
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All)]
    public class Attribute1Attribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class Attribute2Attribute : Attribute
    {
    }

    [Attribute1]
    [Attribute2]
    [WhenBuildingAttributes.Test]
    public class HasManyAttributes
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldAllowMultipleAttributes()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("MultiplesAttribute", attr =>
                    {
                        attr.SetMultipleAllowed();
                    });

                    sc.AddClass("HasMultipleAttributes", cls =>
                    {
                        cls.AddAttribute(attribute);
                        cls.AddAttribute(attribute);
                        cls.AddAttribute(attribute);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MultiplesAttribute : Attribute
    {
    }

    [Multiples]
    [Multiples]
    [Multiples]
    public class HasMultipleAttributes
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldAllowNonInheritedAttributes()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("NotInheritedAttribute", attr =>
                    {
                        attr.SetNotInherited();
                    });

                    sc.AddClass("HasAttribute", cls =>
                    {
                        cls.AddAttribute(attribute);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class NotInheritedAttribute : Attribute
    {
    }

    [NotInherited]
    public class HasAttribute
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldApplyAnAttributeToAField()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("ThisIsAFieldAttribute", attr =>
                    {
                        attr.SetValidOn(AttributeTargets.Field);
                        attr.SetNotInherited();
                    });

                    sc.AddClass("HasFieldAttribute", cls =>
                    {
                        cls.AddField<int>("IntValue", f =>
                        {
                            f.AddAttribute(attribute);
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class ThisIsAFieldAttribute : Attribute
    {
    }

    public class HasFieldAttribute
    {
        [ThisIsAField]
        public int IntValue;
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldApplyAnAttributeToAProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var attribute = sc.AddAttribute("FancyAttribute", attr =>
                    {
                        attr.SetValidOn(AttributeTargets.Property);
                        attr.SetMultipleAllowed();
                        attr.SetSealed();
                    });

                    sc.AddClass("HasPropertyAttribute", cls =>
                    {
                        cls.AddProperty<string>("SomeValue", p =>
                        {
                            p.AddAttribute(attribute);
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FancyAttribute : Attribute
    {
    }

    public class HasPropertyAttribute
    {
        [Fancy]
        public string SomeValue { get; set; }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldApplyAnAttributeToAMethod()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("HasMethodAttribute", cls =>
                    {
                        cls.AddMethod("Nowt", m =>
                        {
                            m.AddAttribute<TestAttribute>();
                            m.SetStatic();
                            m.SetBody(Empty());
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class HasMethodAttribute
    {
        [WhenBuildingAttributes.Test]
        public static void Nowt()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        #region Helper Members

        [AttributeUsage(Struct)]
        public class BaseStructAttribute : Attribute
        {
        }

        public class TestAttribute : Attribute
        {
        }

        public class CtorHelperAttribute : Attribute
        {
            public CtorHelperAttribute(DateTime value)
            {
                Value = value;
            }

            public CtorHelperAttribute(string value)
            {
                Value = value;
            }

            public object Value { get; }
        }

        #endregion
    }
}
