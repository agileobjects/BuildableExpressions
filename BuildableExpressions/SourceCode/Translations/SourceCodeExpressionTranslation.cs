namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions;
    using ReadableExpressions.Translations;

    internal class SourceCodeExpressionTranslation :
        ExpressionTranslation,
        ISourceCodeTranslationContext
    {
        private readonly SourceCodeAnalysis _analysis;

        public SourceCodeExpressionTranslation(
            SourceCodeExpression expression,
            SourceCodeTranslationSettings settings)
            : this(SourceCodeAnalysis.For(expression, settings), settings)
        {
        }

        private SourceCodeExpressionTranslation(
            SourceCodeAnalysis analysis,
            TranslationSettings settings)
            : base(analysis, settings)
        {
            _analysis = analysis;
        }

        #region ISourceCodeTranslationContext Members

        IList<string> ISourceCodeTranslationContext.RequiredNamespaces
            => _analysis.RequiredNamespaces;

        #endregion
    }
}