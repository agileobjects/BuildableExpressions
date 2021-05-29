namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
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

        public static void WriteWithTrailingNewLineTo(
            this AttributeSetTranslation translatable,
            TranslationWriter writer)
        {
            translatable.WriteWithTrailingSeparatorTo(
                writer,
                w => w.WriteNewLineToTranslation());
        }

        public static void WriteWithTrailingSeparatorTo(
            this AttributeSetTranslation attributesTranslation,
            TranslationWriter writer,
            Action<TranslationWriter> separatorWriter)
        {
            if (attributesTranslation.IsEmpty)
            {
                return;
            }

            attributesTranslation.WriteTo(writer, separatorWriter);
            separatorWriter.Invoke(writer);
        }
    }
}
