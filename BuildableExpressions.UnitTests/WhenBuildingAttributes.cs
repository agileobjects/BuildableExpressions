namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using Xunit;

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
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
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
    public abstract class ParentAttribute : Attribute
    {
    }

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

        #region Helper Members

        [AttributeUsage(AttributeTargets.Struct)]
        public class BaseStructAttribute : Attribute
        {
        }

        #endregion
    }
}
