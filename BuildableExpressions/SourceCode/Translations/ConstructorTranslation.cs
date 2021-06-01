namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class ConstructorTranslation : ITranslation
    {
        private readonly AttributeSetTranslation _attributesTranslation;
        private readonly ITranslatable _summaryTranslation;
        private readonly ITranslation _definitionTranslation;
        private readonly bool _hasChainedCtorCall;
        private readonly ITranslatable _chainedCtorCallTranslation;
        private readonly bool _hasBody;
        private readonly ITranslatable _bodyTranslation;

        public ConstructorTranslation(
            ConstructorExpression ctorExpression,
            ITranslationContext context)
        {
            _attributesTranslation = AttributeSetTranslation.For(ctorExpression, context);
            _summaryTranslation = SummaryTranslation.For(ctorExpression, context);

            var parametersTranslation = AttributedParameterSetDefinitionTranslation
                .For(ctorExpression, context);

            _definitionTranslation = new ConstructorDefinitionTranslation(
                ctorExpression,
                parametersTranslation,
                context.Settings);

            _hasChainedCtorCall = ctorExpression.HasChainedConstructorCall;

            var translationSize =
                _attributesTranslation.TranslationSize +
                _summaryTranslation.TranslationSize +
                _definitionTranslation.TranslationSize;

            var formattingSize =
                _attributesTranslation.FormattingSize +
                _summaryTranslation.FormattingSize +
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
                _attributesTranslation.GetIndentSize() +
                _summaryTranslation.GetIndentSize() +
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
                _attributesTranslation.GetLineCount() +
                _summaryTranslation.GetLineCount() +
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
            _attributesTranslation.WriteMultiLineTo(writer);
            _summaryTranslation.WriteTo(writer);
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