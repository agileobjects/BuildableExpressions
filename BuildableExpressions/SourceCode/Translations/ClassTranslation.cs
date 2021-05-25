namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class ClassTranslation : ITranslation
    {
        private const string _classString = "class ";

        private readonly TypeTranslation _typeTranslation;
        private readonly ClassExpressionBase _class;
        private readonly string _modifier;
        private readonly ITranslatable _baseTypeTranslation;

        public ClassTranslation(ClassExpressionBase @class, ITranslationContext context)
        {
            _class = @class;
            _typeTranslation = new TypeTranslation(@class, _classString, context);

            var translationSize = _typeTranslation.TranslationSize;
            var formattingSize = _typeTranslation.FormattingSize;

            if (@class.IsStatic)
            {
                _modifier = "static ";
            }
            else if (@class.IsAbstract)
            {
                _modifier = "abstract ";
            }
            else if (@class.IsSealed)
            {
                _modifier = "sealed ";
            }

            if (_modifier != null)
            {
                translationSize += _modifier.Length;
            }

            if (@class.BaseTypeClassExpression != null)
            {
                IType baseType = @class.BaseTypeClassExpression;
                _baseTypeTranslation = context.GetTranslationFor(baseType);
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
            _typeTranslation.WriteTypeDeclarationTo(writer, _modifier);
            _typeTranslation.WriteTypeListTo(writer, _baseTypeTranslation);
            _typeTranslation.WriteTypeBodyTo(writer);
        }
    }
}
