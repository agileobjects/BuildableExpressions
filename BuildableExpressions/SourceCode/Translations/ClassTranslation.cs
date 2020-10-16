namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

    internal class ClassTranslation : ITranslation
    {
        private const string _staticString = "static ";
        private const string _abstractString = "abstract ";
        private const string _classString = "class ";
        private const string _structString = "struct ";

        private readonly string _visibility;
        private readonly ClassExpression _class;
        private readonly string _typeString;
        private readonly int _typeListCount;
        private readonly IList<ITranslation> _typeList;
        private readonly ITranslatable _summary;
        private readonly IList<ITranslation> _methods;
        private readonly int _methodCount;

        public ClassTranslation(
            ClassExpression @class,
            ITranslationContext context)
        {
            _class = @class;
            _summary = SummaryTranslation.For(@class.Summary, context);
            _visibility = @class.Visibility.ToString().ToLowerInvariant();
            _typeString = @class.IsValueType ? _structString : _classString;
            _typeListCount = @class.Interfaces.Count;

            var hasBaseType = @class.BaseType != typeof(object);

            if (hasBaseType)
            {
                ++_typeListCount;
            }

            _methodCount = @class.Methods.Count;
            _methods = new ITranslation[_methodCount];

            var translationSize =
                _summary.TranslationSize +
                _visibility.Length + 1 +
                _typeString.Length +
                @class.Name.Length +
                6; // <- for opening and closing braces

            if (@class.IsStatic)
            {
                translationSize += _staticString.Length;
            }
            else if (@class.IsAbstract)
            {
                translationSize += _abstractString.Length;
            }

            var keywordFormattingSize = context.GetKeywordFormattingSize();

            var formattingSize =
                _summary.FormattingSize +
                keywordFormattingSize; // <- for accessibility + 'class'

            if (_typeListCount != 0)
            {
                translationSize += 3; // <- for ' : '
                translationSize += ((_typeListCount - 1) * 2); // <- for separators
                _typeList = new ITranslation[_typeListCount];
                var typeIndex = 0;

                if (hasBaseType)
                {
                    var baseType = _typeList[0] = context.GetTranslationFor(@class.BaseType);
                    translationSize += baseType.TranslationSize;
                    formattingSize += baseType.FormattingSize;
                    typeIndex = 1;
                }

                foreach (var @interface in @class.Interfaces)
                {
                    var interfaceTranslation = context.GetTranslationFor(@interface);
                    _typeList[typeIndex] = interfaceTranslation;
                    translationSize += interfaceTranslation.TranslationSize;
                    formattingSize += interfaceTranslation.FormattingSize;
                    ++typeIndex;
                }
            }

            if (_methodCount != 0)
            {
                for (var i = 0; ;)
                {
                    var method = _methods[i] = context.GetTranslationFor(@class.Methods[i]);
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

        public ExpressionType NodeType => _class.NodeType;

        public Type Type => _class.Type;

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
                indentSize += _methods[i].GetIndentSize();

                ++i;

                if (i == _methodCount)
                {
                    return indentSize;
                }
            }
        }

        public int GetLineCount()
        {
            var lineCount = _summary.GetLineCount();

            if (_methodCount == 0)
            {
                return lineCount;
            }

            for (var i = 0; ;)
            {
                lineCount += _methods[i].GetLineCount();

                ++i;

                if (i == _methodCount)
                {
                    return lineCount;
                }
            }
        }

        public void WriteTo(TranslationWriter writer)
        {
            _summary.WriteTo(writer);

            var declaration = _visibility + " ";

            if (_class.IsStatic)
            {
                declaration += _staticString;
            }
            else if (_class.IsAbstract)
            {
                declaration += _abstractString;
            }

            declaration += _typeString;

            writer.WriteKeywordToTranslation(declaration);
            writer.WriteTypeNameToTranslation(_class.Name);

            if (_typeListCount != 0)
            {
                writer.WriteToTranslation(" : ");

                for (var i = 0; ;)
                {
                    _typeList[i].WriteTo(writer);
                    ++i;

                    if (i == _typeListCount)
                    {
                        break;
                    }

                    writer.WriteToTranslation(", ");
                }
            }

            writer.WriteOpeningBraceToTranslation();

            if (_methodCount != 0)
            {
                for (var i = 0; ;)
                {
                    _methods[i].WriteTo(writer);
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
}
