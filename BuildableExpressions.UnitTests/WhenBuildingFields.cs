namespace AgileObjects.BuildableExpressions.UnitTests
{
    using BuildableExpressions.SourceCode;
    using Common;
    using Xunit;

    public class WhenBuildingFields
    {
        [Fact]
        public void ShouldBuildAPublicInstanceReadWriteClassField()
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

        [Fact]
        public void ShouldBuildAPrivateStaticReadWriteStructField()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct(str =>
                    {
                        str.AddField<int>("MyField", f =>
                        {
                            f.SetVisibility(MemberVisibility.Private);
                            f.SetStatic();
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct GeneratedExpressionStruct
    {
        private static int MyField;
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}