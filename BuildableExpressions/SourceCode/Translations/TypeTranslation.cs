namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using BuildableExpressions.Extensions;
    using Generics;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TypeTranslation
    {
        private readonly TypeExpression _type;
        private readonly bool _isGenericType;
        private readonly string _typeString;
        private readonly ITranslatable _summaryTranslation;
        private readonly string _visibility;
        private readonly IList<ITranslation> _interfaceTypeTranslations;
        private readonly int _interfaceTypeCount;
        private readonly int _memberCount;
        private readonly int _fieldCount;
        private readonly IList<ITranslation> _fieldTranslations;
        private readonly int _propertyCount;
        private readonly IList<ITranslation> _propertyTranslations;
        private readonly int _methodCount;
        private readonly IList<ITranslation> _methodTranslations;
        private readonly ITranslatable _genericParametersTranslation;
        private readonly ITranslatable _genericParameterConstraintsTranslation;

        public TypeTranslation(
            TypeExpression type,
            string typeString,
            ITranslationContext context)
        {
            _type = type;
            _isGenericType = type.IsGeneric;
            _typeString = typeString;
            _summaryTranslation = SummaryTranslation.For(type.Summary, context);
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

            _fieldCount = type.FieldExpressions.Count;

            if (_fieldCount != 0)
            {
                _memberCount += _fieldCount;
                _fieldTranslations = new ITranslation[_fieldCount];

                for (var i = 0; ;)
                {
                    var fieldTranslation = context.GetTranslationFor(type.FieldExpressions[i]);
                    var field = _fieldTranslations[i] = fieldTranslation;
                    translationSize += field.TranslationSize;
                    formattingSize += field.FormattingSize;

                    ++i;

                    if (i == _fieldCount)
                    {
                        break;
                    }

                    translationSize += 2; // <- for new line
                }
            }

            _propertyCount = type.PropertyExpressions.Count;

            if (_propertyCount != 0)
            {
                _memberCount += _propertyCount;
                _propertyTranslations = new ITranslation[_propertyCount];

                for (var i = 0; ;)
                {
                    var propertyTranslation = context.GetTranslationFor(type.PropertyExpressions[i]);
                    var property = _propertyTranslations[i] = propertyTranslation;
                    translationSize += property.TranslationSize;
                    formattingSize += property.FormattingSize;

                    ++i;

                    if (i == _propertyCount)
                    {
                        break;
                    }

                    translationSize += 2; // <- for new line
                }
            }

            _methodCount = type.MethodExpressions.Count;

            if (_methodCount != 0)
            {
                _memberCount += _methodCount;
                _methodTranslations = new ITranslation[_methodCount];

                for (var i = 0; ;)
                {
                    var methodTranslation = context.GetTranslationFor(type.MethodExpressions[i]);
                    var method = _methodTranslations[i] = methodTranslation;
                    translationSize += method.TranslationSize;
                    formattingSize += method.FormattingSize;

                    ++i;

                    if (i == _methodCount)
                    {
                        break;
                    }

                    translationSize += 2; // <- for new line
                }
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
        {
            var indentSize = 0;

            if (_isGenericType)
            {
                indentSize +=
                    _genericParametersTranslation.GetIndentSize() +
                    _genericParameterConstraintsTranslation.GetIndentSize();
            }

            if (_methodCount == 0)
            {
                return indentSize;
            }

            for (var i = 0; ;)
            {
                indentSize += _methodTranslations[i].GetIndentSize();

                ++i;

                if (i == _methodCount)
                {
                    return indentSize;
                }
            }
        }

        public int GetLineCount()
        {
            var lineCount =
                _summaryTranslation.GetLineCount();

            if (_isGenericType)
            {
                lineCount +=
                    _genericParametersTranslation.GetLineCount() +
                    _genericParameterConstraintsTranslation.GetLineCount();
            }

            if (_methodCount == 0)
            {
                return lineCount;
            }

            for (var i = 0; ;)
            {
                lineCount += _methodTranslations[i].GetLineCount();

                ++i;

                if (i == _methodCount)
                {
                    return lineCount;
                }
            }
        }

        public void WriteTypeDeclarationTo(
            TranslationWriter writer,
            string modifiers = null)
        {
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

        public void WriteMembersTo(TranslationWriter writer)
        {
            if (_memberCount == 0)
            {
                writer.WriteNewLineToTranslation();
                writer.WriteToTranslation('{');
                writer.WriteNewLineToTranslation();
                writer.WriteToTranslation('}');
                return;
            }

            writer.WriteOpeningBraceToTranslation();

            WriteFieldsTo(writer);
            WritePropertiesTo(writer);
            WriteMethodsTo(writer);

            writer.WriteClosingBraceToTranslation();
        }

        private void WriteFieldsTo(TranslationWriter writer)
        {
            if (_fieldCount != 0)
            {
                WriteTo(
                    writer,
                    _fieldCount,
                    _fieldTranslations,
                    separateWithBlankLine: false);
            }
        }

        private void WritePropertiesTo(TranslationWriter writer)
        {
            if (_propertyCount != 0)
            {
                WriteTo(writer, _propertyCount, _propertyTranslations);
            }
        }

        private void WriteMethodsTo(TranslationWriter writer)
        {
            if (_methodCount != 0)
            {
                WriteTo(writer, _memberCount, _methodTranslations);
            }
        }

        private static void WriteTo(
            TranslationWriter writer,
            int memberCount,
            IList<ITranslation> memberTranslations,
            bool separateWithBlankLine = true)
        {
            if (memberCount == 0)
            {
                return;
            }

            for (var i = 0; ;)
            {
                memberTranslations[i].WriteTo(writer);
                ++i;

                if (i == memberCount)
                {
                    break;
                }

                writer.WriteNewLineToTranslation();

                if (separateWithBlankLine)
                {
                    writer.WriteNewLineToTranslation();
                }
            }
        }
    }
}