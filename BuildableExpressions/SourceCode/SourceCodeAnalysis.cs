namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions;

    internal class SourceCodeAnalysis : ExpressionAnalysis
    {
        private readonly NamespaceAnalysis _namespaceAnalysis;

        private SourceCodeAnalysis(SourceCodeTranslationSettings settings)
            : base(settings)
        {
            _namespaceAnalysis = new NamespaceAnalysis(settings);
        }

        #region Factory Method

        public static SourceCodeAnalysis For(
            SourceCodeExpression expression,
            SourceCodeTranslationSettings settings)
        {
            var analysis = new SourceCodeAnalysis(settings);
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

                case (ExpressionType)SourceCodeExpressionType.Class:
                    return VisitAndConvert((ClassExpression)expression);

                case (ExpressionType)SourceCodeExpressionType.Method:
                    return VisitAndConvert((MethodExpression)expression);

                default:
                    return base.VisitAndConvert(expression);
            }
        }

        private SourceCodeExpression VisitAndConvert(SourceCodeExpression sourceCode)
        {
            sourceCode.Finalise(VisitAndConvert(
                sourceCode.Classes,
                c => (ClassExpression)VisitAndConvert((Expression)c)));

            return sourceCode;
        }

        private ClassExpression VisitAndConvert(ClassExpression @class)
        {
            _namespaceAnalysis.Visit(@class);

            VisitAndConvert(
                @class.Methods,
                m => (MethodExpression)VisitAndConvert((Expression)m));

            return @class;
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