namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

    internal class SourceCodeTranslation : ITranslation
    {
        private const string _using = "using ";
        private const string _namespace = "namespace ";

        private readonly ITranslation _header;
        private readonly bool _hasHeader;
        private readonly IList<string> _namespaces;
        private readonly int _namespaceCount;
        private readonly bool _hasNamespace;
        private readonly SourceCodeExpression _sourceCode;
        private readonly IList<ITranslation> _types;
        private readonly int _typeCount;

        public SourceCodeTranslation(
            ConfiguredSourceCodeExpression sourceCode,
            ITranslationContext context)
        {
            _hasHeader = sourceCode.Header != null;
            _namespaces = sourceCode.Analysis.RequiredNamespaces;
            _namespaceCount = sourceCode.Analysis.RequiredNamespaces.Count;
            _hasNamespace = !string.IsNullOrWhiteSpace(sourceCode.Namespace);
            _sourceCode = sourceCode;
            _typeCount = sourceCode.TypeExpressions.Count;
            _types = new ITranslation[_typeCount];

            var keywordFormattingSize = context.GetKeywordFormattingSize();

            var translationSize = 6; // <- for opening and closing braces
            var formattingSize = 0;

            if (_hasHeader)
            {
                _header = context.GetTranslationFor(sourceCode.Header);
                translationSize += _header.TranslationSize;
                formattingSize += _header.FormattingSize;
            }

            if (_hasNamespace)
            {
                translationSize += _namespace.Length + sourceCode.Namespace.Length;
                formattingSize += keywordFormattingSize;
            }

            if (_namespaceCount != 0)
            {
                for (var i = 0; ;)
                {
                    translationSize += _namespaces[i].Length;
                    formattingSize += keywordFormattingSize; // <- for using

                    ++i;

                    if (i == _namespaceCount)
                    {
                        break;
                    }

                    translationSize += 2; // <- for new line
                }
            }

            for (var i = 0; ;)
            {
                var @class = _types[i] = context.GetTranslationFor(sourceCode.TypeExpressions[i]);

                translationSize += @class.TranslationSize;
                formattingSize += @class.FormattingSize;

                ++i;

                if (i == _typeCount)
                {
                    break;
                }

                translationSize += 2; // <- for new line
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public ExpressionType NodeType => _sourceCode.NodeType;

        public Type Type => _sourceCode.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
        {
            var indentSize = 0;

            for (var i = 0; ;)
            {
                indentSize += _types[i].GetIndentSize();

                ++i;

                if (i == _typeCount)
                {
                    return indentSize;
                }
            }
        }

        public int GetLineCount()
        {
            var lineCount =
                _namespaceCount +
                3; // <- for braces and namespace declaration

            if (_hasHeader)
            {
                lineCount += _header.GetLineCount();
            }

            if (_namespaceCount > 0)
            {
                ++lineCount; // <- for gap between namespaces and namespace declaration
            }

            for (var i = 0; ;)
            {
                lineCount += _types[i].GetLineCount();

                ++i;

                if (i == _typeCount)
                {
                    return lineCount;
                }
            }
        }

        public void WriteTo(TranslationWriter writer)
        {
            if (_hasHeader)
            {
                _header.WriteTo(writer);
                writer.WriteNewLineToTranslation();
                writer.WriteNewLineToTranslation();
            }

            if (_namespaceCount != 0)
            {
                for (var i = 0; i < _namespaceCount; ++i)
                {
                    writer.WriteKeywordToTranslation(_using);
                    writer.WriteToTranslation(_namespaces[i]);
                    writer.WriteToTranslation(';');
                    writer.WriteNewLineToTranslation();
                }

                writer.WriteNewLineToTranslation();
            }

            if (_hasNamespace)
            {
                writer.WriteKeywordToTranslation(_namespace);
                writer.WriteToTranslation(_sourceCode.Namespace);
                writer.WriteOpeningBraceToTranslation();
            }

            for (var i = 0; ;)
            {
                _types[i].WriteTo(writer);

                ++i;

                if (i == _typeCount)
                {
                    break;
                }

                writer.WriteNewLineToTranslation();
                writer.WriteNewLineToTranslation();
            }

            if (_hasNamespace)
            {
                writer.WriteClosingBraceToTranslation();
            }
        }
    }
}
