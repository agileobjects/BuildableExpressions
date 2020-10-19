namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using ReadableExpressions.Translations;
    using static SourceCodeTranslationSettings;

    internal class SourceCodeExpressionTranslation : ExpressionTranslation
    {
        public SourceCodeExpressionTranslation(SourceCodeExpression expression)
            : base(expression.Analysis, Settings)
        {
        }
    }
}