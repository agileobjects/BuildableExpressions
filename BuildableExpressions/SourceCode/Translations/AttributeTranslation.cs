namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using static ReadableExpressions.Translations.Formatting.TokenType;

    internal class AttributeTranslation : ITranslation
    {
        private readonly AttributeExpression _attribute;
        private readonly string _attributeName;

        public AttributeTranslation(AttributeExpression attribute, ITranslationContext context)
        {
            _attribute = attribute;
            _attributeName = attribute.Name.Substring(0, attribute.Name.Length - "Attribute".Length);

            TranslationSize = _attributeName.Length + 2;
            FormattingSize = context.GetFormattingSize(TypeName);
        }

        public Type Type => _attribute.Type;

        public ExpressionType NodeType => _attribute.NodeType;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize() => 0;

        public int GetLineCount() => 1;

        public void WriteTo(TranslationWriter writer)
        {
            writer.WriteToTranslation('[');
            writer.WriteTypeNameToTranslation(_attributeName);
            writer.WriteToTranslation(']');
        }
    }
}
