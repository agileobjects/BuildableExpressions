namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;

    public class WhenBuildingInterfaces
    {
        [Fact]
        public void ShouldBuildAnEmptyInterface()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IMarker", itf => { });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public interface IMarker { }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}
