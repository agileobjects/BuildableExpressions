namespace AgileObjects.BuildableExpressions.SourceCode
{
    using ReadableExpressions.Translations.Reflection;

    internal static class PropertyExtensions
    {
        public static bool IsAutoProperty(this IProperty property)
            => property.Getter?.HasBody != true && property.Setter?.HasBody != true;
    }
}