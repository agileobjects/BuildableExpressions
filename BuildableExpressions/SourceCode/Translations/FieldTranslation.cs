namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class FieldTranslation : FieldDefinitionTranslation
    {
        private readonly ITranslatable _valueTranslation;

        public FieldTranslation(
            FieldExpression fieldExpression,
            ITranslationContext context)
            : base(
                fieldExpression,
                includeDeclaringType: false,
                context.Settings)
        {
            if (fieldExpression.InitialValue == null)
            {
                return;
            }

            _valueTranslation = context.GetTranslationFor(fieldExpression.InitialValue);
            TranslationSize = base.TranslationSize + 3 + _valueTranslation.TranslationSize;
            FormattingSize = base.FormattingSize + _valueTranslation.FormattingSize;
        }

        public override int TranslationSize { get; }

        public override int FormattingSize { get; }

        public override void WriteTo(TranslationWriter writer)
        {
            if (_valueTranslation == null)
            {
                base.WriteTo(writer);
                return;
            }

            WriteDefinitionTo(writer);
            writer.WriteToTranslation(" = ");
            _valueTranslation.WriteTo(writer);
            writer.WriteToTranslation(';');
        }
    }
}