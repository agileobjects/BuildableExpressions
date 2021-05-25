namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using static ReadableExpressions.Translations.Formatting.TokenType;

    internal class AttributeSetTranslation : ITranslatable
    {
        private readonly int _attributeCount;
        private readonly IList<ITranslatable> _attributeTranslations;

        public AttributeSetTranslation(
            IList<AppliedAttribute> attributes,
            ITranslationContext context)
        {
            _attributeCount = attributes.Count;
            _attributeTranslations = new ITranslatable[_attributeCount];

            var translationSize = 0;
            var formattingSize = 0;

            for (var i = 0; i < _attributeCount; ++i)
            {
                var translation =
                    new AppliedAttributeTranslation(attributes[i], context);

                translationSize += translation.TranslationSize;
                formattingSize += translation.FormattingSize;
                _attributeTranslations[i] = translation;
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
                _attributeTranslations[i].WriteTo(writer);
                ++i;

                if (i == _attributeCount)
                {
                    break;
                }

                writer.WriteNewLineToTranslation();
            }
        }

        private class AppliedAttributeTranslation : ITranslatable
        {
            private readonly IList<string> _attributeNameParts;
            private readonly ParameterSetTranslation _parameters;

            public AppliedAttributeTranslation(
                AppliedAttribute attribute,
                ITranslationContext context)
            {
                var typeName = attribute.AttributeExpression.GetFriendlyName();
                var attributeName = typeName.Substring(0, typeName.Length - "Attribute".Length);

                _attributeNameParts = ((IType)attribute.AttributeExpression).IsNested
                    ? attributeName.Split('.')
                    : new[] { attributeName };

                var typeNameFormattingSize = context.GetFormattingSize(TypeName);

                var translationSize = attributeName.Length + 2;
                var formattingSize = _attributeNameParts.Count * typeNameFormattingSize;

                if (attribute.ArgumentsAccessor != null)
                {
                    _parameters = ParameterSetTranslation
                        .For(
                            attribute.ConstructorExpression,
                            attribute.Arguments,
                            context)
                        .WithParentheses();

                    translationSize += _parameters.TranslationSize;
                    formattingSize += _parameters.FormattingSize;
                }

                TranslationSize = translationSize;
                FormattingSize = formattingSize;
            }

            public int TranslationSize { get; }

            public int FormattingSize { get; }

            public int GetIndentSize() => _parameters?.GetIndentSize() ?? 0;

            public int GetLineCount() => _parameters?.GetLineCount() ?? 1;

            public void WriteTo(TranslationWriter writer)
            {
                writer.WriteToTranslation('[');

                for (var i = 0; ;)
                {
                    writer.WriteTypeNameToTranslation(_attributeNameParts[i]);
                    ++i;

                    if (i == _attributeNameParts.Count)
                    {
                        break;
                    }

                    writer.WriteToTranslation('.');
                }

                _parameters?.WriteTo(writer);
                writer.WriteToTranslation(']');
            }
        }
    }
}