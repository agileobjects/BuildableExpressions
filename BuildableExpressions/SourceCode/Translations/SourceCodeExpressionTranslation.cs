namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using ReadableExpressions.Translations;
    using static SourceCodeTranslationSettings;

    internal class SourceCodeExpressionTranslation : ExpressionTranslation
    {
        public SourceCodeExpressionTranslation(ConfiguredSourceCodeExpression expression)
            : base(expression.Analysis, Settings)
        {
        }
    }
}