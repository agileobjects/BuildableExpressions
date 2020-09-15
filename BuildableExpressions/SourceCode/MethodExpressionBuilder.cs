﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Linq.Expressions;
    using Api;
    using ReadableExpressions;

    internal class MethodExpressionBuilder : IMethodExpressionSettings
    {
        private CommentExpression _summary;

        public MethodExpressionBuilder(string name, Expression body)
        {
            Name = name;
            Definition = body.ToLambdaExpression();
        }

        public string Name { get; }

        public LambdaExpression Definition { get; }

        public IMethodExpressionSettings WithSummary(string summary)
            => WithSummary(ReadableExpression.Comment(summary));

        public IMethodExpressionSettings WithSummary(CommentExpression summary)
        {
            _summary = summary;
            return this;
        }

        public MethodExpression Build(
            ClassExpression parent,
            SourceCodeTranslationSettings settings)
        {
            return MethodExpression
                .For(parent, Name, _summary, Definition, settings);
        }
    }
}