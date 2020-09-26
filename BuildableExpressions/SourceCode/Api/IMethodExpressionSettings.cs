namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using ReadableExpressions;

    /// <summary>
    /// Provides configuration options to control aspects of <see cref="MethodExpression"/> creation.
    /// </summary>
    public interface IMethodExpressionSettings
    {
        /// <summary>
        /// Set the visibility of the <see cref="MethodExpression"/> being built to the given
        /// <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to use for the <see cref="MethodExpression"/> being
        /// built.
        /// </param>
        /// <returns>These <see cref="IMethodExpressionSettings"/>, to support a fluent interface.</returns>
        IMethodExpressionSettings WithVisibility(MemberVisibility visibility);

        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/> being built.
        /// </summary>
        /// <param name="summary">The summary documentation of the <see cref="MethodExpression"/> being built.</param>
        /// <returns>These <see cref="IMethodExpressionSettings"/>, to support a fluent interface.</returns>
        IMethodExpressionSettings WithSummary(string summary);

        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/> being built.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MethodExpression"/> being built.
        /// </param>
        /// <returns>These <see cref="IMethodExpressionSettings"/>, to support a fluent interface.</returns>
        IMethodExpressionSettings WithSummary(CommentExpression summary);
    }
}