namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using System.Linq;
    using BuildableExpressions.Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Formatting;
    using static System.Environment;

    internal class SummaryTranslation : ITranslatable
    {
        private static readonly ITranslatable _empty = new SummaryTranslation();

        private const string _tripleSlash = "/// ";
        private const string _summaryStart = _tripleSlash + "<summary>";
        private const string _summaryEnd = _tripleSlash + "</summary>";

        private readonly int _lineCount;
        private readonly IList<string> _textLines;

        private SummaryTranslation()
        {
        }

        private SummaryTranslation(IList<string> textLines, ITranslationContext context)
        {
            _lineCount = textLines.Count;
            _textLines = textLines.ProjectToArray(line => _tripleSlash + line);

            TranslationSize =
                _summaryStart.Length + NewLine.Length +
                textLines.Sum(line => line.Length + NewLine.Length) +
                _summaryEnd.Length + NewLine.Length;

            FormattingSize =
                GetLineCount() * context.GetFormattingSize(TokenType.Comment);
        }

        #region Factory Method

        public static ITranslatable For(CommentExpression summary, ITranslationContext context)
        {
            return summary?.Comment.TextLines.Any() == true
                ? new SummaryTranslation(summary.Comment.TextLines.ToList(), context)
                : _empty;
        }

        #endregion

        public int TranslationSize { get; }

        public int FormattingSize { get; }

        public int GetIndentSize() => 0;

        public int GetLineCount() => _lineCount + 2;

        public void WriteTo(TranslationWriter writer)
        {
            if (_lineCount == 0)
            {
                return;
            }

            writer.WriteToTranslation(_summaryStart, TokenType.Comment);
            writer.WriteNewLineToTranslation();

            for (var i = 0; i < _lineCount; ++i)
            {
                writer.WriteToTranslation(_textLines[i], TokenType.Comment);
                writer.WriteNewLineToTranslation();
            }

            writer.WriteToTranslation(_summaryEnd, TokenType.Comment);
            writer.WriteNewLineToTranslation();
        }
    }
}