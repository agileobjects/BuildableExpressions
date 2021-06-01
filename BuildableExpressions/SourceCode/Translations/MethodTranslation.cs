﻿namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class MethodTranslation : ITranslation
    {
        private readonly MethodExpression _methodExpression;
        private readonly AttributeSetTranslation _attributesTranslation;
        private readonly ITranslatable _summaryTranslation;
        private readonly ITranslatable _definitionTranslation;
        private readonly bool _methodHasBody;
        private readonly ITranslation _bodyTranslation;

        public MethodTranslation(
            MethodExpression methodExpression,
            ITranslationContext context)
        {
            _methodExpression = methodExpression;
            _attributesTranslation = AttributeSetTranslation.For(methodExpression, context);
            _summaryTranslation = SummaryTranslation.For(methodExpression, context);

            var parametersTranslation = AttributedParameterSetDefinitionTranslation
                .For(methodExpression, context);

            _definitionTranslation = new MethodDefinitionTranslation(
                methodExpression,
                parametersTranslation,
                includeDeclaringType: false,
                context.Settings);

            var translationSize =
                _attributesTranslation.TranslationSize +
                _summaryTranslation.TranslationSize +
                _definitionTranslation.TranslationSize;

            var formattingSize =
                _attributesTranslation.FormattingSize +
                _summaryTranslation.FormattingSize +
                _definitionTranslation.FormattingSize;

            _methodHasBody = _methodExpression.HasBody;

            if (_methodHasBody)
            {
                var bodyCodeBlock = context
                    .GetCodeBlockTranslationFor(methodExpression.Body)
                    .WithBraces()
                    .WithTermination();

                if (methodExpression.HasReturnType())
                {
                    bodyCodeBlock.WithReturnKeyword();
                }

                translationSize += bodyCodeBlock.TranslationSize;
                formattingSize += bodyCodeBlock.FormattingSize;
                _bodyTranslation = bodyCodeBlock;
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public ExpressionType NodeType => _methodExpression.NodeType;

        public Type Type => _methodExpression.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
        {
            var indentSize =
                _attributesTranslation.GetIndentSize() +
                _summaryTranslation.GetIndentSize() +
                _definitionTranslation.GetIndentSize();

            if (_methodHasBody)
            {
                indentSize += _bodyTranslation.GetIndentSize();
            }

            return indentSize;
        }

        public int GetLineCount()
        {
            var lineCount =
                _attributesTranslation.GetLineCount() +
                _summaryTranslation.GetLineCount() +
                _definitionTranslation.GetLineCount();

            if (_methodHasBody)
            {
                lineCount += _bodyTranslation.GetLineCount();
            }

            return lineCount;
        }

        public void WriteTo(TranslationWriter writer)
        {
            _attributesTranslation.WriteMultiLineTo(writer);
            _summaryTranslation.WriteTo(writer);
            _definitionTranslation.WriteTo(writer);

            if (_methodHasBody)
            {
                _bodyTranslation.WriteTo(writer);
            }
            else
            {
                writer.WriteToTranslation(';');
            }
        }
    }
}
