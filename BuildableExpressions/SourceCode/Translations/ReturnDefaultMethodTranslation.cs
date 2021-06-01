namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class ReturnDefaultMethodTranslation : ITranslation
    {
        private readonly IMethod _method;
        private readonly MethodDefinitionTranslation _definitionTranslation;
        private readonly string _body;

        public ReturnDefaultMethodTranslation(
            IMethod method,
            ITranslationContext context)
        {
            _method = method;

            _definitionTranslation = new MethodDefinitionTranslation(
                method,
                ParameterSetDefinitionTranslation.For(method, context.Settings),
                includeDeclaringType: false,
                context.Settings);

            _body = !method.IsAbstract
                ? method.HasReturnType()
                    ? "{return default(" + method.Type.GetFriendlyName() + ");}"
                    : "{}" : ";";

            TranslationSize =
                _definitionTranslation.TranslationSize +
                _body.Length;
        }

        public ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Method;

        public Type Type => _method.Type.AsType();

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