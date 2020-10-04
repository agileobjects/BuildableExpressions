namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions.Translations;

    internal interface ISourceCodeTranslationContext : ITranslationContext
    {
        /// <summary>
        /// Gets the namespaces required by the translated Expression.
        /// </summary>
        IList<string> RequiredNamespaces { get; }
    }
}