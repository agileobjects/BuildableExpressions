namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;

    internal class StructTranslation : ITranslation
    {
        private const string _structString = "struct ";

        private readonly TypeTranslation _typeTranslation;
        private readonly StructExpression _struct;

        public StructTranslation(StructExpression @struct, ITranslationContext context)
        {
            _struct = @struct;
            _typeTranslation = new TypeTranslation(@struct, _structString, context);
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
            _typeTranslation.WriteTypeBodyTo(writer);
        }
    }
}