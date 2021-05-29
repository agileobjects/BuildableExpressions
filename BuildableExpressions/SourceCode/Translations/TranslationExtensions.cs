namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal static class TranslationExtensions
    {
        public static string GetAccessibilityForTranslation(this IMember member)
        {
            var accessibility = member.GetAccessibility();

            if (!string.IsNullOrEmpty(accessibility))
            {
                accessibility += ' ';
            }

            return accessibility;
        }

        public static string GetModifiersForTranslation(this IComplexMember member)
        {
            var modifiers = member.GetModifiers();

            if (!string.IsNullOrEmpty(modifiers))
            {
                modifiers += ' ';
            }

            return modifiers;
        }

        public static void WriteWithNewLineIfNotEmptyTo(
            this IPotentialEmptyTranslatable translatable,
            TranslationWriter writer)
        {
            if (translatable.IsEmpty)
            {
                return;
            }

            translatable.WriteTo(writer);
            writer.WriteNewLineToTranslation();
        }
    }
}
