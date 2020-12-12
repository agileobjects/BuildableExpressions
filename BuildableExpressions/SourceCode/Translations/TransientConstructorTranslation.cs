namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TransientConstructorTranslation : ITranslation
    {
        private readonly ConstructorExpression _ctorExpression;
        private readonly ITranslation _definitionTranslation;

        public TransientConstructorTranslation(
            ConstructorExpression ctorExpression,
            ITranslationContext context)
        {
            _ctorExpression = ctorExpression;

            _definitionTranslation =
                new ConstructorDefinitionTranslation(ctorExpression, context.Settings);
        }

        public ExpressionType NodeType => _ctorExpression.NodeType;

        public Type Type => _ctorExpression.Type;

        public int TranslationSize => _definitionTranslation.TranslationSize + 2;

        public int FormattingSize => 0;

        public int GetIndentSize() => _definitionTranslation.GetIndentSize();

        public int GetLineCount() => _definitionTranslation.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            _definitionTranslation.WriteTo(writer);
            writer.WriteToTranslation("{}");
        }
    }
}