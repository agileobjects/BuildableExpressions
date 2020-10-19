namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;

    public class WhenBuildingBclReflectionObjects
    {
        [Fact]
        public void ShouldAddReferenceAssembliesToSourceCode()
        {
            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls => { });
                });

            sourceCode.ToSourceCode();

            sourceCode.ReferencedAssemblies.ShouldBeEmpty();
        }
    }
}
