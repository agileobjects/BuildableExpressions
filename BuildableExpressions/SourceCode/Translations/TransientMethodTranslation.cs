namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TransientMethodTranslation : ITranslation
    {
        private readonly MethodExpression _methodExpression;
        private readonly MethodDefinitionTranslation _definitionTranslation;
        private readonly string _body;

        public TransientMethodTranslation(
            MethodExpression methodExpression,
            ITranslationContext context)
        {
            _methodExpression = methodExpression;

            _definitionTranslation = new MethodDefinitionTranslation(
                methodExpression,
                includeDeclaringType: false,
                context.Settings);

            _body = methodExpression.HasBody
                ? methodExpression.HasReturnType()
                    ? "{return default(" + methodExpression.ReturnType.GetFriendlyName() + ");}"
                    : "{}" : ";";

            TranslationSize =
                _definitionTranslation.TranslationSize +
                _body.Length;
        }

        public ExpressionType NodeType => _methodExpression.NodeType;

        public Type Type => _methodExpression.Type;

        public int TranslationSize { get; }

        public int FormattingSize
            => _definitionTranslation.FormattingSize;

        public int GetIndentSize()
            => _definitionTranslation.GetIndentSize();

        public int GetLineCount()
            => _definitionTranslation.GetLineCount();

        public void WriteTo(TranslationWriter writer)
        {
            _definitionTranslation.WriteTo(writer);
            writer.WriteToTranslation(_body);
        }
    }
}