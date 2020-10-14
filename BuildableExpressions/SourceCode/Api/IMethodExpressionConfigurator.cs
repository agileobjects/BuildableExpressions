namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/>.
    /// </summary>
    public interface IMethodExpressionConfigurator
    {
        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="summary">The summary documentation of the <see cref="MethodExpression"/>.</param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithSummary(string summary);

        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithSummary(CommentExpression summary);

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithVisibility(MemberVisibility visibility);

        /// <summary>
        /// Mark the <see cref="MethodExpression"/> as static.
        /// </summary>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator AsStatic();

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to give the <see cref="MethodExpression"/>.</param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator Named(string name);

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithGenericParameter(GenericParameterExpression parameter);
    }
}