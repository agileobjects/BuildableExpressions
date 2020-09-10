namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Linq.Expressions;
    using ReadableExpressions;

    internal class MethodExpressionBuilder
    {
        private readonly CommentExpression _summary;

        public MethodExpressionBuilder(
            string name,
            CommentExpression summary,
            Expression body)
        {
            Name = name;
            _summary = summary;
            Definition = body.ToLambdaExpression();
        }

        public string Name { get; }

        public LambdaExpression Definition { get; }

        public MethodExpression Build(
            ClassExpression parent,
            SourceCodeTranslationSettings settings)
        {
            return MethodExpression
                .For(parent, Name, _summary, Definition, settings);
        }
    }
}