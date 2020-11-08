namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Formatting;
    using static SourceCodeTranslationSettings;

    internal class EnumTranslation : ITranslation
    {
        private const string _enumString = "enum ";

        private readonly EnumExpression _enum;
        private readonly ITranslatable _summaryTranslation;
        private readonly string _visibility;
        private readonly int _memberCount;
        private readonly KeyValuePair<string, string>[] _members;

        public EnumTranslation(EnumExpression @enum, ITranslationContext context)
        {
            _enum = @enum;
            _summaryTranslation = SummaryTranslation.For(@enum.Summary, context);
            _visibility = @enum.Visibility.ToString().ToLowerInvariant();

            var translationSize =
                _summaryTranslation.TranslationSize +
                _visibility.Length +
                _enumString.Length +
                @enum.Name.Length +
                6; // <- for opening and closing braces

            var keywordFormattingSize = context.GetKeywordFormattingSize();

            var formattingSize =
                _summaryTranslation.FormattingSize +
                keywordFormattingSize + // <- for accessibility, modifiers + 'struct'
                keywordFormattingSize;  // <- for enum name

            _memberCount = @enum.MembersAccessor.Count;
            _members = new KeyValuePair<string, string>[_memberCount];

            var i = 0;

            foreach (var nameAndValue in @enum.MembersAccessor)
            {
                var name = nameAndValue.Key;
                var value = nameAndValue.Value.ToString();

                translationSize +=
                    name.Length + value.Length +
                    5; // <- for ' = ' and new line

                formattingSize +=
                    keywordFormattingSize; // <- for numeric value

                _members[i] = new KeyValuePair<string, string>(name, value);
                ++i;
            }

            TranslationSize = translationSize;
            FormattingSize = formattingSize;
        }

        public ExpressionType NodeType => _enum.NodeType;

        public Type Type => _enum.Type;

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize()
            => _memberCount * Settings.IndentLength;

        public int GetLineCount()
            => _enum.MembersAccessor.Count + 3;

        public void WriteTo(TranslationWriter writer)
        {
            _summaryTranslation.WriteTo(writer);

            writer.WriteKeywordToTranslation(_visibility + " " + _enumString);
            writer.WriteTypeNameToTranslation(_enum.Name);

            writer.WriteOpeningBraceToTranslation();

            for (var i = 0; ;)
            {
                var member = _members[i];

                writer.WriteToTranslation(member.Key);
                writer.WriteToTranslation(" = ");
                writer.WriteToTranslation(member.Value, TokenType.Numeric);

                ++i;

                if (i == _memberCount)
                {
                    break;
                }

                writer.WriteToTranslation(',');
                writer.WriteNewLineToTranslation();
            }

            writer.WriteClosingBraceToTranslation();
        }
    }
}