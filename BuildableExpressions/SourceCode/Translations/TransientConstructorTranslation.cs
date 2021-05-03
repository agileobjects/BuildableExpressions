namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TransientConstructorTranslation : ITranslation
    {
        private readonly ConstructorExpression _ctorExpression;
        private readonly ITranslation _definitionTranslation;
        private readonly bool _hasChainedCtorCall;
        private readonly ITranslatable _chainedCtorCallTranslation;

        public TransientConstructorTranslation(
            ConstructorExpression ctorExpression,
            ITranslationContext context)
        {
            _ctorExpression = ctorExpression;

            _definitionTranslation =
                new ConstructorDefinitionTranslation(ctorExpression, context.Settings);

            _hasChainedCtorCall = 
                ctorExpression.HasChainedConstructorCall && 
                ctorExpression.ChainedConstructorCall.Arguments.Count != 0;

            var translationSize = _definitionTranslation.TranslationSize;

            if (_hasChainedCtorCall)
            {
                _chainedCtorCallTranslation = new ChainedConstructorCallTranslation(
                    ctorExpression.ChainedConstructorCall,
                    context);

                translationSize += _chainedCtorCallTranslation.TranslationSize;
            }

            TranslationSize = translationSize;
        }

        public ExpressionType NodeType => _ctorExpression.NodeType;

        public Type Type => _ctorExpression.Type;

        public int TranslationSize { get; }

        public int FormattingSize => 0;

        public int GetIndentSize()
        {
            var indentSize =
                _definitionTranslation.GetIndentSize();

            if (_hasChainedCtorCall)
            {
                indentSize += _chainedCtorCallTranslation.GetIndentSize();
            }

            return indentSize;
        }

        public int GetLineCount()
        {
            var lineCount = _definitionTranslation.GetLineCount();

            if (_hasChainedCtorCall)
            {
                lineCount += _chainedCtorCallTranslation.GetLineCount();
            }

            return lineCount;
        }

        public void WriteTo(TranslationWriter writer)
        {
            _definitionTranslation.WriteTo(writer);

            if (_hasChainedCtorCall)
            {
                _chainedCtorCallTranslation.WriteTo(writer);
            }

            writer.WriteToTranslation("{}");
        }
    }
}