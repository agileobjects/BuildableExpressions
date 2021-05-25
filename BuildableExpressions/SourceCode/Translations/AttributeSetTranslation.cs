namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using static ReadableExpressions.Translations.Formatting.TokenType;

    internal class AttributeSetTranslation : ITranslatable
    {
        private readonly int _attributeCount;
        private readonly IList<string> _attributeNames;

        public AttributeSetTranslation(
            IList<AttributeExpression> attributes,
            ITranslationContext context)
        {
            _attributeCount = attributes.Count;
            _attributeNames = new string[_attributeCount];

            var typeNameFormattingSize = context.GetFormattingSize(TypeName);

            var translationSize = 0;
            var formattingSize = 0;

            for (var i = 0; i < _attributeCount; ++i)
            {
                var typeName = attributes[i].GetFriendlyName();
                var attributeName = typeName.Substring(0, typeName.Length - "Attribute".Length);

                translationSize += attributeName.Length;
                formattingSize += typeNameFormattingSize;
                _attributeNames[i] = attributeName;
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
                writer.WriteToTranslation('[');
                writer.WriteTypeNameToTranslation(_attributeNames[i]);
                writer.WriteToTranslation(']');
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