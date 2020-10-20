namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

    internal class TypeTranslation
    {
        private readonly TypeExpression _type;
        private readonly string _typeString;
        private readonly ITranslatable _summaryTranslation;
        private readonly string _visibility;
        private readonly IList<ITranslation> _interfaceTypeTranslations;
        private readonly int _interfaceTypeCount;
        private readonly IList<ITranslation> _methodTranslations;
        private readonly int _methodCount;

        public TypeTranslation(
            TypeExpression type,
            string typeString,
            ITranslationContext context)
        {
            _type = type;
            _typeString = typeString;
            _summaryTranslation = SummaryTranslation.For(type.Summary, context);
            _visibility = type.Visibility.ToString().ToLowerInvariant();
            _interfaceTypeCount = type.InterfaceTypes.Count;

            _methodCount = type.MethodExpressions.Count;
            _methodTranslations = new ITranslation[_methodCount];

            var translationSize =
                _summaryTranslation.TranslationSize +
                _visibility.Length + 1 +
                typeString.Length +
                type.Name.Length +
                6; // <- for opening and closing braces

            var keywordFormattingSize = context.GetKeywordFormattingSize();

            var formattingSize =
                _summaryTranslation.FormattingSize +
                keywordFormattingSize; // <- for accessibility + type name

            if (_interfaceTypeCount != 0)
            {
                translationSize += 3; // <- for ' : '
                translationSize += ((_interfaceTypeCount - 1) * 2); // <- for separators
                _interfaceTypeTranslations = new ITranslation[_interfaceTypeCount];

                for (var i = 0; i < _interfaceTypeCount; ++i)
                {
                    var interfaceTranslation = context.GetTranslationFor(type.InterfaceTypes[i]);
                    _interfaceTypeTranslations[i] = interfaceTranslation;
                    translationSize += interfaceTranslation.TranslationSize;
                    formattingSize += interfaceTranslation.FormattingSize;
                }
            }

            if (_methodCount != 0)
            {
                for (var i = 0; ;)
                {
                    var method = _methodTranslations[i] = context.GetTranslationFor(type.MethodExpressions[i]);
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
            if (_methodCount == 0)
            {
                return 0;
            }

            var indentSize = 0;

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
            var lineCount = _summaryTranslation.GetLineCount();

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

        public void WriteMethodsTo(TranslationWriter writer)
        {
            writer.WriteOpeningBraceToTranslation();

            if (_methodCount != 0)
            {
                for (var i = 0; ;)
                {
                    _methodTranslations[i].WriteTo(writer);
                    ++i;

                    if (i == _methodCount)
                    {
                        break;
                    }

                    writer.WriteNewLineToTranslation();
                    writer.WriteNewLineToTranslation();
                }
            }

            writer.WriteClosingBraceToTranslation();
        }
    }

    internal class StructTranslation : ITranslation
    {
        private const string _structString = "struct ";

        private readonly TypeTranslation _typeTranslation;
        private readonly StructExpression _struct;

        public StructTranslation(StructExpression type, ITranslationContext context)
        {
            _struct = type;
            _typeTranslation = new TypeTranslation(type, _structString, context);
        }

        public ExpressionType NodeType => _struct.NodeType;

        public Type Type => _struct.Type;

        public int TranslationSize => _typeTranslation.TranslationSize;

        public int FormattingSize => _typeTranslation.FormattingSize;

        public int GetIndentSize() => _typeTranslation.GetIndentSize();

        public int GetLineCount() => _typeTranslation.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            _typeTranslation.WriteTypeDeclarationTo(writer);
            _typeTranslation.WriteTypeListTo(writer);
            _typeTranslation.WriteMethodsTo(writer);
        }
    }

    internal class ClassTranslation : ITranslation
    {
        private const string _staticString = "static ";
        private const string _abstractString = "abstract ";
        private const string _classString = "class ";

        private readonly TypeTranslation _typeTranslation;
        private readonly ClassExpression _class;
        private readonly ITranslatable _baseTypeTranslation;

        public ClassTranslation(ClassExpression type, ITranslationContext context)
        {
            _class = type;
            _typeTranslation = new TypeTranslation(type, _classString, context);
            var hasBaseType = type.BaseType != typeof(object);

            var translationSize = _typeTranslation.TranslationSize;
            var formattingSize = _typeTranslation.FormattingSize;

            if (type.IsStatic)
            {
                translationSize += _staticString.Length;
            }
            else if (type.IsAbstract)
            {
                translationSize += _abstractString.Length;
            }

            if (hasBaseType)
            {
                _baseTypeTranslation = context.GetTranslationFor(type.BaseType);
                translationSize += _baseTypeTranslation.TranslationSize;
                formattingSize += _baseTypeTranslation.FormattingSize;
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public ExpressionType NodeType => _class.NodeType;

        public Type Type => _class.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize() => _typeTranslation.GetIndentSize();

        public int GetLineCount() => _typeTranslation.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            var declarationModifiers = string.Empty;

            if (_class.IsStatic)
            {
                declarationModifiers += _staticString;
            }
            else if (_class.IsAbstract)
            {
                declarationModifiers += _abstractString;
            }

            _typeTranslation.WriteTypeDeclarationTo(writer, declarationModifiers);
            _typeTranslation.WriteTypeListTo(writer, _baseTypeTranslation);
            _typeTranslation.WriteMethodsTo(writer);
        }
    }
}
