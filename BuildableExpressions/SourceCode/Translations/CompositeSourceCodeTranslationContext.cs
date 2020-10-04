namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;

    internal class CompositeSourceCodeTranslationContext :
        ISourceCodeTranslationContext
    {
        private readonly NamespaceAnalysis _namespaceAnalysis;
        private readonly ITranslationContext _context;

        public CompositeSourceCodeTranslationContext(
            NamespaceAnalysis namespaceAnalysis,
            ITranslationContext context)
        {
            _namespaceAnalysis = namespaceAnalysis;
            _context = context;
        }

        public TranslationSettings Settings => _context.Settings;

        public IList<string> RequiredNamespaces
            => _namespaceAnalysis.RequiredNamespaces;

        public ICollection<ParameterExpression> InlineOutputVariables
            => _context.InlineOutputVariables;

        public ICollection<ParameterExpression> JoinedAssignmentVariables
            => _context.JoinedAssignmentVariables;

        public bool ShouldBeDeclaredInline(ParameterExpression parameter)
            => _context.ShouldBeDeclaredInline(parameter);

        public bool IsJoinedAssignment(Expression expression)
            => _context.IsJoinedAssignment(expression);

        public bool IsCatchBlockVariable(Expression variable)
            => _context.IsCatchBlockVariable(variable);

        public bool IsReferencedByGoto(LabelTarget labelTarget)
            => _context.IsReferencedByGoto(labelTarget);

        public bool GoesToReturnLabel(GotoExpression @goto)
            => _context.GoesToReturnLabel(@goto);

        public bool IsPartOfMethodCallChain(MethodCallExpression methodCall)
            => _context.IsPartOfMethodCallChain(methodCall);

        public int? GetUnnamedVariableNumberOrNull(ParameterExpression variable)
            => _context.GetUnnamedVariableNumberOrNull(variable);

        public ITranslation GetTranslationFor(Expression expression)
            => _context.GetTranslationFor(expression);
    }
}