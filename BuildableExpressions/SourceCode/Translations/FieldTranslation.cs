namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class FieldTranslation : ITranslation
    {
        private readonly IPotentialEmptyTranslatable _attributesTranslation;
        private readonly ITranslatable _summaryTranslation;
        private readonly FieldDefinitionTranslation _definitionTranslation;
        private readonly ITranslatable _valueTranslation;

        public FieldTranslation(
            FieldExpression fieldExpression,
            ITranslationContext context)
        {
            NodeType = fieldExpression.NodeType;

            _attributesTranslation = AttributeSetTranslation.For(fieldExpression, context);
            _summaryTranslation = SummaryTranslation.For(fieldExpression, context);

            _definitionTranslation = new FieldDefinitionTranslation(
                fieldExpression,
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

            if (fieldExpression.InitialValue != null)
            {
                _valueTranslation = context.GetTranslationFor(fieldExpression.InitialValue);
                translationSize += _valueTranslation.TranslationSize + 3;
                formattingSize += _valueTranslation.FormattingSize;
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        /// <inheritdoc />
        public ExpressionType NodeType { get; }

        public Type Type => _definitionTranslation.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
            => _definitionTranslation.GetIndentSize();

        public int GetLineCount()
            => _definitionTranslation.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            _attributesTranslation.WriteWithNewLineIfNotEmptyTo(writer);
            _summaryTranslation.WriteTo(writer);

            if (_valueTranslation == null)
            {
                _definitionTranslation.WriteTo(writer);
                return;
            }

            _definitionTranslation.WriteDefinitionTo(writer);
            writer.WriteToTranslation(" = ");
            _valueTranslation.WriteTo(writer);
            writer.WriteToTranslation(';');
        }
    }
}