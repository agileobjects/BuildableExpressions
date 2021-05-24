namespace AgileObjects.BuildableExpressions.UnitTests
{
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
    }
}
