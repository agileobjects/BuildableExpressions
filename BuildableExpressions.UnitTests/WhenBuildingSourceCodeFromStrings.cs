namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using ReadableExpressions;
    using Xunit;

    public class WhenBuildingSourceCodeFromStrings
    {
        [Fact]
        public void ShouldBuildAClassWithANamespace()
        {
            const string sourceCode = @"
namespace MagicStringCode.MagicStuff
{
    public class MagicStringClass
    {
        public string GetMessage()
        {
            return ""Hello!"";
        }
    }
}
";
            var translated = BuildableExpression
                .SourceCode(sourceCode)
                .ToSourceCodeString();
            
            translated.ShouldBe(sourceCode.Trim());
        }

        [Fact]
        public void ShouldTranslateAStructWithNoNamespace()
        {
            const string sourceCode = @"
public struct MagicStringStruct
{
    public string GetMessage()
    {
        return ""Hello!"";
    }
}";
            var translated = BuildableExpression
                .SourceCode(sourceCode)
                .ToReadableString();
            
            translated.ShouldBe(sourceCode.Trim());
        }

        [Fact]
        public void ShouldBuildAnInterfaceWithANamespace()
        {
            const string sourceCode = @"
namespace MagicString.Goodness
{
    public interface IMagicString
    {
        public string GetMessage();
    }
}";
            var sourceCodeExpression = 
                BuildableExpression.SourceCode(sourceCode);
            
            sourceCodeExpression.NodeType.ShouldBe((ExpressionType)SourceCodeExpressionType.SourceCode);
            sourceCodeExpression.Type.ShouldBe(typeof(void));
            sourceCodeExpression.Namespace.ShouldBe("MagicString.Goodness");
            sourceCodeExpression.TypeName.ShouldBe("IMagicString");
            sourceCodeExpression.ReferencedAssemblies.ShouldBeEmpty();
            sourceCodeExpression.TypeExpressions.ShouldBeEmpty();
        }
    }
}
