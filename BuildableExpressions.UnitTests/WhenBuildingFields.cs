namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;

    public class WhenBuildingFields
    {
        [Fact]
        public void ShouldBuildAClassReadWriteInstanceField()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddField<string>("MyField");
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string MyField;
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}