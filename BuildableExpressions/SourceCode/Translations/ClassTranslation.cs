namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;

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
            _typeTranslation.WriteMembersTo(writer);
        }
    }
}
