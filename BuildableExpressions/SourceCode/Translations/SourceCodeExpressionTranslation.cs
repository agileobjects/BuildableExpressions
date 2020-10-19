namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions.Translations;
    using static SourceCodeTranslationSettings;

    internal class SourceCodeExpressionTranslation :
        ExpressionTranslation,
        ISourceCodeTranslationContext
    {
        private readonly SourceCodeAnalysis _analysis;

        public SourceCodeExpressionTranslation(SourceCodeExpression expression)
            : this(SourceCodeAnalysis.For(expression))
        {
        }

        private SourceCodeExpressionTranslation(SourceCodeAnalysis analysis)
            : base(analysis, Settings)
        {
            _analysis = analysis;
        }

        #region ISourceCodeTranslationContext Members

        IList<string> ISourceCodeTranslationContext.RequiredNamespaces
            => _analysis.RequiredNamespaces;

        #endregion
    }
}