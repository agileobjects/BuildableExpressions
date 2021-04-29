namespace AgileObjects.BuildableExpressions.UnitTests.Translations
{
    using System;
    using Common;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenTranslatingSourceCode
    {
        [Fact]
        public void ShouldUseANonSourceCodeExpressionAnalysis()
        {
            var sourceCode = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod(
                            "GetNow",
                            Property(null, typeof(DateTime), "Now"))));

            var sourceCodeTranslation = new ExpressionTranslation(
                sourceCode,
                new TestTranslationSettings());

            var translated = sourceCodeTranslation.GetTranslation();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        private class TestTranslationSettings : TranslationSettings
        {
        }

        #endregion
    }
}
