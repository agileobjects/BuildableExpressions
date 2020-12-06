namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;

    internal class InterfaceTranslation : ITranslation
    {
        private const string _interfaceString = "interface ";

        private readonly TypeTranslation _typeTranslation;
        private readonly InterfaceExpression _interface;

        public InterfaceTranslation(InterfaceExpression @interface, ITranslationContext context)
        {
            _interface = @interface;
            _typeTranslation = new TypeTranslation(@interface, _interfaceString, context);
        }

        public ExpressionType NodeType => _interface.NodeType;

        public Type Type => _interface.Type;

        public int TranslationSize => _typeTranslation.TranslationSize;

        public int FormattingSize => _typeTranslation.FormattingSize;

        public int GetIndentSize() => _typeTranslation.GetIndentSize();

        public int GetLineCount() => _typeTranslation.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            _typeTranslation.WriteTypeDeclarationTo(writer);
            _typeTranslation.WriteTypeListTo(writer);
            _typeTranslation.WriteBodyTo(writer);
        }
    }
}