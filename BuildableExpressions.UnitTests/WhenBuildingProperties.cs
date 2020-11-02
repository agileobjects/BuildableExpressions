namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;
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
                            p.SetGetter(g => g.SetVisibility(Public));
                            p.SetSetter(s => s.SetVisibility(Private));
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
    }
}
