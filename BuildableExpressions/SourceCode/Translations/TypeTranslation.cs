namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using Generics;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using static SourceCodeExpressionType;

    internal class TypeTranslation
    {
        private readonly TypeExpression _type;
        private readonly bool _hasAttributes;
        private readonly AttributeSetTranslation _attributesTranslation;
        private readonly bool _isGenericType;
        private readonly string _typeString;
        private readonly ITranslatable _summaryTranslation;
        private readonly string _visibility;
        private readonly IList<ITranslation> _interfaceTypeTranslations;
        private readonly int _interfaceTypeCount;
        private readonly ITranslatable _genericParametersTranslation;
        private readonly ITranslatable _genericParameterConstraintsTranslation;
        private readonly int _memberCount;
        private readonly IList<ITranslation> _memberTranslations;

        public TypeTranslation(
            TypeExpression type,
            string typeString,
            ITranslationContext context)
        {
            _type = type;
            _hasAttributes = type.AttributesAccessor != null;
            _isGenericType = type.IsGeneric;
            _typeString = typeString;
            _summaryTranslation = SummaryTranslation.For(type, context);
            _visibility = type.Visibility.ToString().ToLowerInvariant();

            var translationSize =
                _summaryTranslation.TranslationSize +
                _visibility.Length + 1 +
                typeString.Length +
                type.Name.Length +
                6; // <- for opening and closing braces

            var keywordFormattingSize = context.GetKeywordFormattingSize();

            var formattingSize =
                _summaryTranslation.FormattingSize +
                keywordFormattingSize + // <- for accessibility, modifiers + type string
                keywordFormattingSize;  // <- for type name

            if (_hasAttributes)
            {
                _attributesTranslation = AttributeSetTranslation.For(type, context);

                translationSize += _attributesTranslation.TranslationSize;
                formattingSize += _attributesTranslation.FormattingSize;
            }

            if (_isGenericType)
            {
                var genericParameters = type.GenericParameters
                    .ProjectToArray<GenericParameterExpression, IGenericParameter>(p => p);

                var settings = context.Settings;

                _genericParametersTranslation =
                    new GenericParameterSetDefinitionTranslation(genericParameters, settings);

                _genericParameterConstraintsTranslation =
                    new GenericParameterSetConstraintsTranslation(genericParameters, settings);

                translationSize +=
                    _genericParametersTranslation.TranslationSize +
                    _genericParameterConstraintsTranslation.TranslationSize;

                formattingSize +=
                    _genericParametersTranslation.FormattingSize +
                    _genericParameterConstraintsTranslation.FormattingSize;
            }

            _interfaceTypeCount = type.InterfaceTypeExpressions.Count;

            if (_interfaceTypeCount != 0)
            {
                translationSize += 3; // <- for ' : '
                translationSize += ((_interfaceTypeCount - 1) * 2); // <- for separators
                _interfaceTypeTranslations = new ITranslation[_interfaceTypeCount];

                for (var i = 0; i < _interfaceTypeCount; ++i)
                {
                    var interfaceType = (IType)type.InterfaceTypeExpressions[i];
                    var interfaceTranslation = context.GetTranslationFor(interfaceType);
                    _interfaceTypeTranslations[i] = interfaceTranslation;
                    translationSize += interfaceTranslation.TranslationSize;
                    formattingSize += interfaceTranslation.FormattingSize;
                }
            }

            _memberCount = type.MemberExpressionsAccessor.Count;

            if (_memberCount == 0)
            {
                TranslationSize = translationSize;
                FormattingSize = formattingSize;
                return;
            }

            _memberTranslations = new ITranslation[_memberCount];

            for (var i = 0; ;)
            {
                var memberTranslation = context.GetTranslationFor(type.MemberExpressions[i]);
                var member = _memberTranslations[i] = memberTranslation;
                translationSize += member.TranslationSize;
                formattingSize += member.FormattingSize;

                ++i;

                if (i == _memberCount)
                {
                    break;
                }

                translationSize += 2; // <- for new line
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
        {
            var indentSize = 0;

            if (_hasAttributes)
            {
                indentSize += _attributesTranslation.GetIndentSize();
            }

            if (_isGenericType)
            {
                indentSize +=
                    _genericParametersTranslation.GetIndentSize() +
                    _genericParameterConstraintsTranslation.GetIndentSize();
            }

            if (_memberCount == 0)
            {
                return indentSize;
            }

            for (var i = 0; ;)
            {
                indentSize += _memberTranslations[i].GetIndentSize();
                ++i;

                if (i == _memberCount)
                {
                    return indentSize;
                }
            }
        }

        public int GetLineCount()
        {
            var lineCount =
                _summaryTranslation.GetLineCount();

            if (_hasAttributes)
            {
                lineCount += _attributesTranslation.GetLineCount();
            }

            if (_isGenericType)
            {
                lineCount +=
                    _genericParametersTranslation.GetLineCount() +
                    _genericParameterConstraintsTranslation.GetLineCount();
            }

            if (_memberCount == 0)
            {
                return lineCount;
            }

            for (var i = 0; ;)
            {
                lineCount += _memberTranslations[i].GetLineCount();
                ++i;

                if (i == _memberCount)
                {
                    return lineCount;
                }
            }
        }

        public void WriteTypeDeclarationTo(
            TranslationWriter writer,
            string modifiers = null)
        {
            if (_hasAttributes)
            {
                _attributesTranslation.WriteMultiLineTo(writer);
            }

            _summaryTranslation.WriteTo(writer);

            var declaration = _visibility + " " + modifiers + _typeString;

            writer.WriteKeywordToTranslation(declaration);
            writer.WriteTypeNameToTranslation(_type.Name);

            if (_isGenericType)
            {
                _genericParametersTranslation.WriteTo(writer);
                _genericParameterConstraintsTranslation.WriteTo(writer);
            }
        }

        public void WriteTypeListTo(
            TranslationWriter writer,
            ITranslatable extraTypeTranslation = null)
        {
            var hasExtraType = extraTypeTranslation != null;
            var hasNoInterfaces = _interfaceTypeCount == 0;

            if (!hasExtraType && hasNoInterfaces)
            {
                return;
            }

            writer.WriteToTranslation(" : ");

            if (hasExtraType)
            {
                extraTypeTranslation.WriteTo(writer);

                if (hasNoInterfaces)
                {
                    return;
                }

                writer.WriteToTranslation(", ");
            }

            for (var i = 0; ;)
            {
                _interfaceTypeTranslations[i].WriteTo(writer);
                ++i;

                if (i == _interfaceTypeCount)
                {
                    break;
                }

                writer.WriteToTranslation(", ");
            }
        }

        public void WriteTypeBodyTo(TranslationWriter writer)
        {
            writer.WriteOpeningBraceToTranslation();

            if (_memberCount == 0)
            {
                writer.WriteClosingBraceToTranslation(startOnNewLine: false);
                return;
            }

            WriteMembersTo(writer);
            writer.WriteClosingBraceToTranslation();
        }

        private void WriteMembersTo(TranslationWriter writer)
        {
            if (_memberCount == 0)
            {
                return;
            }

            for (var i = 0; ;)
            {
                var memberTranslation = _memberTranslations[i];
                memberTranslation.WriteTo(writer);
                ++i;

                if (i == _memberCount)
                {
                    break;
                }

                writer.WriteNewLineToTranslation();

                if (IsField(memberTranslation))
                {
                    var nextMemberTranslation = _memberTranslations[i];

                    if (IsField(nextMemberTranslation))
                    {
                        continue;
                    }
                }

                writer.WriteNewLineToTranslation();
            }
        }

        private static bool IsField(ITranslation memberTranslation)
            => memberTranslation.NodeType == (ExpressionType)Field;
    }
}