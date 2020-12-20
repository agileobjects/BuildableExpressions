namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/>.
    /// </summary>
    public interface ITypeExpressionConfigurator
    {
        /// <summary>
        /// Gets a Type object for the <see cref="TypeExpression"/>. The returned Type is lazily
        /// and dynamically generated when this property is accessed, and is built from the state
        /// of the <see cref="TypeExpression"/> at the time the property is accessed.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Set the summary documentation of the <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="TypeExpression"/>.
        /// </param>
        void SetSummary(CommentExpression summary);

        /// <summary>
        /// Set the visibility of the <see cref="TypeExpression"/> to the given
        /// <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="TypeVisibility"/> to use for the <see cref="TypeExpression"/>.
        /// </param>
        void SetVisibility(TypeVisibility visibility);
    }
}