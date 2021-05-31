﻿namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

    internal class ChainedConstructorCallTranslation : ITranslation
    {
        private const string _baseKeyword = "base";
        private const string _thisKeyword = "this";

        private readonly ITranslationContext _context;
        private readonly string _keyword;
        private readonly ChainedConstructorCallExpression _chainedCtorCallExpression;
        private readonly ITranslatable _argumentTranslations;

        public ChainedConstructorCallTranslation(
            ChainedConstructorCallExpression chainedCtorCallExpression,
            ITranslationContext context)
        {
            _context = context;

            _keyword =
                chainedCtorCallExpression.TargetConstructor.DeclaringTypeExpression ==
                chainedCtorCallExpression.CallingConstructor.DeclaringTypeExpression
                    ? _thisKeyword : _baseKeyword;

            _chainedCtorCallExpression = chainedCtorCallExpression;

            _argumentTranslations = ParameterSetTranslation
                .For(
                    chainedCtorCallExpression.TargetConstructor,
                    chainedCtorCallExpression.Arguments,
                    context)
                .WithParentheses();

            TranslationSize =
                Environment.NewLine.Length +
                2 + _keyword.Length +
                _argumentTranslations.TranslationSize;

            FormattingSize =
                context.GetKeywordFormattingSize() +
                _argumentTranslations.FormattingSize;
        }

        public ExpressionType NodeType => _chainedCtorCallExpression.NodeType;

        public Type Type => _chainedCtorCallExpression.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize() => _context.Settings.IndentLength;

        public int GetLineCount() => _argumentTranslations.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            writer.WriteNewLineToTranslation();
            writer.WriteToTranslation(": ");
            writer.WriteKeywordToTranslation(_keyword);
            _argumentTranslations.WriteTo(writer);
        }
    }
}