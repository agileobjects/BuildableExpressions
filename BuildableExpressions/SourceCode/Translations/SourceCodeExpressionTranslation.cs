namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Interfaces;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Interfaces;
    using static System.Linq.Expressions.ExpressionType;
    using static SourceCodeExpressionType;

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

        public override ITranslation GetTranslationFor(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch (expression.NodeType)
            {
                case Call when expression is BuildableMethodCallExpression methodCall:
                    return MethodCallTranslation.For(methodCall.Method, methodCall.Arguments, this);

                case (ExpressionType)SourceCode:
                    return new SourceCodeTranslation((SourceCodeExpression)expression, this);

                case (ExpressionType)Class:
                    return new ClassTranslation((ClassExpression)expression, this);

                case (ExpressionType)Method:
                    return new MethodTranslation((MethodExpression)expression, this);
            }

            return base.GetTranslationFor(expression);
        }

        #endregion
    }
}