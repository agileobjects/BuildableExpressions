namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions.Translations;

    internal class AttributeSetTranslation : ITranslatable
    {
        private readonly int _attributeCount;
        private readonly IList<ITranslatable> _attributes;

        public AttributeSetTranslation(
            IList<AttributeExpression> attributes,
            ITranslationContext context)
        {
            _attributeCount = attributes.Count;
            _attributes = new ITranslatable[_attributeCount];

            var translationSize = 0;
            var formattingSize = 0;

            for (var i = 0; i < _attributeCount; ++i)
            {
                var translation = context.GetTranslationFor(attributes[i]);

                translationSize += translation.TranslationSize;
                formattingSize += translation.FormattingSize;
                _attributes[i] = translation;
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize() => 0;

        public int GetLineCount() => _attributeCount;

        public void WriteTo(TranslationWriter writer)
        {
            for (var i = 0; ;)
            {
                _attributes[i].WriteTo(writer);
                ++i;

                if (i == _attributeCount)
                {
                    break;
                }

                writer.WriteNewLineToTranslation();
            }
        }
    }
}