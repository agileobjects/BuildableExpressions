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
        private readonly int _attributeCount;
        private readonly IList<ITranslatable> _attributeTranslations;

        private AttributeSetTranslation(
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

        public static IPotentialEmptyTranslatable For(TypeExpression type, ITranslationContext context)
            => For(type.AttributesAccessor, context);

        public static IPotentialEmptyTranslatable For(MemberExpressionBase member, ITranslationContext context)
            => For(member.AttributesAccessor, context);

        private static IPotentialEmptyTranslatable For(
            IList<AppliedAttribute> attributes, 
            ITranslationContext context)
        {
            return attributes?.Any() == true
                ? new AttributeSetTranslation(attributes, context)
                : EmptyTranslatable.Instance;
        }

        #endregion

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public bool IsEmpty => false;

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
            private const string _allowMultiple = ", AllowMultiple = ";
            private const string _inherited = ", Inherited = ";
            private const string _true = "true";
            private const string _false = "false";

            private readonly bool _writeAllowMultiple;
            private readonly bool _writeNotInherited;
            private readonly IList<string> _attributeNameParts;
            private readonly ParameterSetTranslation _parameters;

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
                    _parameters = ParameterSetTranslation
                        .For(
                            attribute.ConstructorExpression,
                            attribute.Arguments,
                            context)
                        .WithoutParentheses();

                    translationSize += _parameters.TranslationSize;
                    formattingSize += _parameters.FormattingSize;
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

                if (_parameters == null)
                {
                    writer.WriteToTranslation(']');
                    return;
                }

                writer.WriteToTranslation('(');
                _parameters.WriteTo(writer);
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