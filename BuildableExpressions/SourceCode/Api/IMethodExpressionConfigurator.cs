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
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MethodExpression"/>.
        /// </param>
        void SetSummary(CommentExpression summary);

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="MethodExpression"/>.
        /// </param>
        void SetVisibility(MemberVisibility visibility);

        /// <summary>
        /// Mark the <see cref="MethodExpression"/> as static.
        /// </summary>
        void SetStatic();

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="parameters">The <see cref="GenericParameterExpression"/>s to add.</param>
        void AddGenericParameters(params GenericParameterExpression[] parameters);
    }
}