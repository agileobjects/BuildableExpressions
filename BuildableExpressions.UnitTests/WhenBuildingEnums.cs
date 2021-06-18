namespace AgileObjects.BuildableExpressions.UnitTests
{
    using BuildableExpressions.SourceCode;
    using Common;
    using Xunit;

    public class WhenBuildingEnums
    {
        [Fact]
        public void ShouldBuildAnEnumWithAutoValues()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddEnum("Numbers", "Zero", "One", "Two", "Three");
                })
                .ToSourceCodeString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public enum Numbers
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInternalEnumWithGivenValues()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddEnum("OddNumbers", enm =>
                    {
                        enm.SetVisibility(TypeVisibility.Internal);

                        enm.AddMember("Two", 2);
                        enm.AddMember("Four", 4);
                        enm.AddMember("Six", 6);
                    });
                })
                .ToSourceCodeString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    internal enum OddNumbers
    {
        Two = 2,
        Four = 4,
        Six = 6
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }
    }
}
