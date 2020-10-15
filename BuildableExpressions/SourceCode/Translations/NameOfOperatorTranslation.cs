namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using ReadableExpressions.Translations;

    internal class NameOfOperatorTranslation : UnaryOperatorTranslationBase
    {
        public NameOfOperatorTranslation(
            ITranslatable operandTranslation,
            ITranslationContext context)
            : base("nameof", operandTranslation, context.Settings)
        {
        }

        public override Type Type => typeof(string);
    }
}
