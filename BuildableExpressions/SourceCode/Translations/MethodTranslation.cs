namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class MethodTranslation : ITranslation
    {
        private readonly MethodExpression _method;
        private readonly ITranslatable _summary;
        private readonly ITranslatable _definitionTranslation;
        private readonly bool _methodHasBody;
        private readonly ITranslation _bodyTranslation;

        public MethodTranslation(MethodExpression method, ITranslationContext context)
        {
            _method = method;
            _summary = SummaryTranslation.For(method.Summary, context);

            _definitionTranslation = new MethodDefinitionTranslation(
                method,
                includeDeclaringType: false,
                context.Settings);

            var translationSize =
                _summary.TranslationSize +
                _definitionTranslation.TranslationSize;

            var formattingSize =
                _summary.FormattingSize +
                _definitionTranslation.FormattingSize;

            _methodHasBody = _method.HasBody;

            if (_methodHasBody)
            {
                var bodyCodeBlock = context
                    .GetCodeBlockTranslationFor(method.Body)
                    .WithBraces()
                    .WithTermination();

                if (method.HasReturnType())
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

        public ExpressionType NodeType => _method.NodeType;

        public Type Type => _method.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
        {
            var indentSize = _definitionTranslation.GetIndentSize();

            if (_methodHasBody)
            {
                indentSize += _bodyTranslation.GetIndentSize();
            }

            return indentSize;
        }

        public int GetLineCount()
        {
            var lineCount = _definitionTranslation.GetLineCount();

            if (_methodHasBody)
            {
                lineCount += _bodyTranslation.GetLineCount();
            }

            return lineCount;
        }

        public void WriteTo(TranslationWriter writer)
        {
            _summary.WriteTo(writer);
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
