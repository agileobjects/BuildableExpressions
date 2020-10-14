namespace AgileObjects.BuildableExpressions.UnitTests.SourceCode.Translations
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
            var sourceCode = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(
                            Property(null, typeof(DateTime), "Now"),
                            m => m.Named("GetNow"))));

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
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        private class TestTranslationSettings : TranslationSettings
        {
        }

        #endregion
    }
}
