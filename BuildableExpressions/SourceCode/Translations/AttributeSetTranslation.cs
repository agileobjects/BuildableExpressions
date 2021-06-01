namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using static ReadableExpressions.Translations.Formatting.TokenType;

    internal class AttributeSetTranslation : IPotentialEmptyTranslatable
    {
        private static readonly AttributeSetTranslation _empty = new();

        private readonly int _attributeCount;
        private readonly IList<ITranslatable> _attributeTranslations;

        private AttributeSetTranslation()
        {
        }

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

        #region Factory Methods

        public static AttributeSetTranslation For(TypeExpression type, ITranslationContext context)
            => For(type.AttributesAccessor, context);

        public static AttributeSetTranslation For(MemberExpressionBase member, ITranslationContext context)
            => For(member.AttributesAccessor, context);

        private static AttributeSetTranslation For(
            IList<AppliedAttribute> attributes,
            ITranslationContext context)
        {
            return attributes?.Any() == true
                ? new AttributeSetTranslation(attributes, context)
                : _empty;
        }

        #endregion

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public bool IsEmpty => _attributeCount == 0;

        public int GetIndentSize() => 0;

        public int GetLineCount() => _attributeCount;

        public void WriteTo(TranslationWriter writer)
            => WriteTo(writer, w => w.WriteNewLineToTranslation());

        public void WriteTo(
            TranslationWriter writer,
            Action<TranslationWriter> separatorWriter)
        {
            if (IsEmpty)
            {
                return;
            }

            for (var i = 0; ;)
            {
                _attributeTranslations[i].WriteTo(writer);
                ++i;

                if (i == _attributeCount)
                {
                    break;
                }

                separatorWriter.Invoke(writer);
            }
        }

        private class AppliedAttributeTranslation : ITranslatable
        {
            private const string _allowMultiple = ", AllowMultiple = ";
            private const string _inherited = ", Inherited = ";
            private const string _true = "true";
            private const string _false = "false";

            private readonly bool _writeAllowMultiple;
            private readonly bool _writeNotInherited;
            private readonly IList<string> _attributeNameParts;
            private readonly ITranslatable _arguments;

            public AppliedAttributeTranslation(
                AppliedAttribute attribute,
                ITranslationContext context)
            {
                var attributeExpression = attribute.AttributeExpression;
                var typeName = attributeExpression.GetFriendlyName();
                var attributeName = typeName.Substring(0, typeName.Length - "Attribute".Length);

                _attributeNameParts = ((IType)attributeExpression).IsNested
                    ? attributeName.Split('.')
                    : new[] { attributeName };

                var typeNameFormattingSize = context.GetFormattingSize(TypeName);

                var translationSize = attributeName.Length + 2;
                var formattingSize = _attributeNameParts.Count * typeNameFormattingSize;

                if (attribute.ArgumentsAccessor != null)
                {
                    _arguments = ParameterSetTranslation
                        .For(
                            attribute.ConstructorExpression,
                            attribute.Arguments,
                            context)
                        .WithoutParentheses();

                    translationSize += _arguments.TranslationSize;
                    formattingSize += _arguments.FormattingSize;
                }

                if (IsUsageAttribute(attributeExpression))
                {
                    if (attribute.AllowMultiple)
                    {
                        _writeAllowMultiple = true;
                        translationSize += _allowMultiple.Length + _true.Length;
                        formattingSize += context.GetKeywordFormattingSize();
                    }

                    if (!attribute.Inherited)
                    {
                        _writeNotInherited = true;
                        translationSize += _inherited.Length + _false.Length;
                        formattingSize += context.GetKeywordFormattingSize();
                    }
                }

                TranslationSize = translationSize;
                FormattingSize = formattingSize;
            }

            #region Setup

            private static bool IsUsageAttribute(IType attribute)
            {
                return
                    attribute is TypedAttributeExpression typedAttribute &&
                    typedAttribute.Type == typeof(AttributeUsageAttribute);
            }

            #endregion

            public int TranslationSize { get; }

            public int FormattingSize { get; }

            public int GetIndentSize() => _arguments?.GetIndentSize() ?? 0;

            public int GetLineCount() => _arguments?.GetLineCount() ?? 1;

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

                    writer.WriteDotToTranslation();
                }

                if (_arguments == null)
                {
                    writer.WriteToTranslation(']');
                    return;
                }

                writer.WriteToTranslation('(');
                _arguments.WriteTo(writer);
                WriteUsageArgumentsIfRequired(writer);
                writer.WriteToTranslation(")]");
            }

            private void WriteUsageArgumentsIfRequired(TranslationWriter writer)
            {
                if (_writeAllowMultiple)
                {
                    writer.WriteToTranslation(_allowMultiple);
                    writer.WriteKeywordToTranslation(_true);
                }

                if (_writeNotInherited)
                {
                    writer.WriteToTranslation(_inherited);
                    writer.WriteKeywordToTranslation(_false);
                }
            }
        }
    }
}