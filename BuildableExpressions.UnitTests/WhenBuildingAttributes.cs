namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using Common;
    using Xunit;
    using static System.AttributeTargets;

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
                            ctor.SetBody(Expression.Empty());
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
        public void ShouldBuildAnAttributeWithUsage()
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

        #region Helper Members

        [AttributeUsage(Struct)]
        public class BaseStructAttribute : Attribute
        {
        }

        #endregion
    }
}
