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
namespace GeneratedExpressionCode
{
    [SpecialFunky]
    public class HasAnAttribute
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildADerivedStructAttribute()
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
    [DerivedFunky]
    public struct AttributedStruct
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }
    }

    #region Helper Members

    [AttributeUsage(AttributeTargets.Struct)]
    public class BaseStructAttribute : Attribute
    {
    }

    #endregion
}
