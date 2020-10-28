namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/>.
    /// </summary>
    public interface ITypeExpressionConfigurator : IGenericParameterConfigurator
    {
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