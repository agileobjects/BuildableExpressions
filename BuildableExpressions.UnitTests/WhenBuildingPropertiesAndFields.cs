namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;

    public class WhenBuildingPropertiesAndFields
    {
        [Fact]
        public void ShouldBuildAClassGetSetAutoProperty()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddProperty("MyProperty", typeof(string));
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
    }
}
