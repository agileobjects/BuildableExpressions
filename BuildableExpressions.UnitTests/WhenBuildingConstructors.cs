namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;

    public class WhenBuildingConstructors
    {
        [Fact]
        public void ShouldBuildAnEmptyPublicConstructor()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor();
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public GeneratedExpressionClass()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}