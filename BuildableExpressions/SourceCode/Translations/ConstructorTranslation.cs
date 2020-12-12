﻿namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class ConstructorTranslation : ITranslation
    {
        private readonly ITranslatable _summary;
        private readonly ITranslation _definitionTranslation;
        private readonly bool _hasChainedCtorCall;
        private readonly ITranslatable _chainedCtorCallTranslation;
        private readonly bool _hasBody;
        private readonly ITranslatable _bodyTranslation;

        public ConstructorTranslation(
            ConstructorExpression ctorExpression,
            ITranslationContext context)
        {
            _summary = SummaryTranslation.For(ctorExpression.Summary, context);

            _definitionTranslation =
                new ConstructorDefinitionTranslation(ctorExpression, context.Settings);

            _hasChainedCtorCall = ctorExpression.HasChainedConstructorCall;

            var translationSize =
                _summary.TranslationSize +
                _definitionTranslation.TranslationSize;

            var formattingSize =
                _summary.FormattingSize +
                _definitionTranslation.FormattingSize;

            if (_hasChainedCtorCall)
            {
                _chainedCtorCallTranslation = new ChainedConstructorCallTranslation(
                    ctorExpression.ChainedConstructorCall,
                    context);

                translationSize += _chainedCtorCallTranslation.TranslationSize;
                formattingSize += _chainedCtorCallTranslation.FormattingSize;
            }

            _hasBody = ctorExpression.HasBody;

            if (_hasBody)
            {
                _bodyTranslation = context
                    .GetCodeBlockTranslationFor(ctorExpression.Body)
                    .WithBraces()
                    .WithTermination();

                translationSize += _bodyTranslation.TranslationSize;
                formattingSize += _bodyTranslation.FormattingSize;
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public ExpressionType NodeType => _definitionTranslation.NodeType;

        public Type Type => _definitionTranslation.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
        {
            var indentSize =
                _summary.GetIndentSize() +
                _definitionTranslation.GetIndentSize();

            if (_hasChainedCtorCall)
            {
                indentSize += _chainedCtorCallTranslation.GetIndentSize();
            }

            if (_hasBody)
            {
                indentSize += _bodyTranslation.GetIndentSize();
            }

            return indentSize;
        }

        public int GetLineCount()
        {
            var lineCount =
                _summary.GetLineCount() +
                _definitionTranslation.GetLineCount();

            if (_hasChainedCtorCall)
            {
                lineCount += _chainedCtorCallTranslation.GetLineCount();
            }

            if (_hasBody)
            {
                lineCount += _bodyTranslation.GetLineCount();
            }

            return lineCount;
        }

        public void WriteTo(TranslationWriter writer)
        {
            _summary.WriteTo(writer);
            _definitionTranslation.WriteTo(writer);

            if (_hasChainedCtorCall)
            {
                _chainedCtorCallTranslation.WriteTo(writer);
            }

            if (_hasBody)
            {
                _bodyTranslation.WriteTo(writer);
            }
            else
            {
                writer.WriteOpeningBraceToTranslation();
                writer.WriteClosingBraceToTranslation(startOnNewLine: false);
            }
        }
    }
}