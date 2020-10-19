namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using static SourceCodeTranslationSettings;

    internal class SourceCodeAnalysis : ExpressionAnalysis
    {
        private readonly NamespaceAnalysis _namespaceAnalysis;

        private SourceCodeAnalysis()
            : base(Settings)
        {
            _namespaceAnalysis = new NamespaceAnalysis();
        }

        #region Factory Method

        public static SourceCodeAnalysis For(SourceCodeExpression expression)
        {
            var analysis = new SourceCodeAnalysis();
            analysis.Analyse(expression);

            return analysis;
        }

        #endregion

        public IList<string> RequiredNamespaces
            => _namespaceAnalysis.RequiredNamespaces;

        protected override Expression VisitAndConvert(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch (expression.NodeType)
            {
                case (ExpressionType)SourceCodeExpressionType.SourceCode:
                    return VisitAndConvert((SourceCodeExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Type:
                    return VisitAndConvert((TypeExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Method:
                    return VisitAndConvert((MethodExpression)expression);

                default:
                    return base.VisitAndConvert(expression);
            }
        }

        private SourceCodeExpression VisitAndConvert(SourceCodeExpression sourceCode)
        {
            sourceCode.Finalise(VisitAndConvert(
                sourceCode.Types,
                c => (TypeExpression)VisitAndConvert((Expression)c)));

            return sourceCode;
        }

        private TypeExpression VisitAndConvert(TypeExpression type)
        {
            _namespaceAnalysis.Visit(type);

            VisitAndConvert(
                type.Methods,
                m => (MethodExpression)VisitAndConvert((Expression)m));

            return type;
        }

        private MethodExpression VisitAndConvert(MethodExpression method)
        {
            var methodAnalysis = MethodExpressionAnalysis.For(method, _namespaceAnalysis);
            Merge(methodAnalysis);
            _namespaceAnalysis.Merge(methodAnalysis.NamespaceAnalysis);

            return method;
        }

        protected override ExpressionAnalysis Finalise()
        {
            _namespaceAnalysis.Finalise();
            return base.Finalise();
        }
    }
}