namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

    internal class ClassTranslation : ITranslation
    {
        private const string _classString = "class ";

        private readonly TypeTranslation _typeTranslation;
        private readonly ClassExpression _class;
        private readonly string _modifier;
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
                _modifier = "static ";
            }
            else if (type.IsAbstract)
            {
                _modifier = "abstract ";
            }
            else if (type.IsSealed)
            {
                _modifier = "sealed ";
            }

            if (_modifier != null)
            {
                translationSize += _modifier.Length;
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
            _typeTranslation.WriteTypeDeclarationTo(writer, _modifier);
            _typeTranslation.WriteTypeListTo(writer, _baseTypeTranslation);
            _typeTranslation.WriteMembersTo(writer);
        }
    }
}
