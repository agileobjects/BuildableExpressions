namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Linq.Expressions;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> or
    /// <see cref="MethodExpression"/>.
    /// </summary>
    public interface IMemberExpressionConfigurator : IAttributableExpressionConfigurator
    {
        /// <summary>
        /// Set the summary documentation of the <see cref="SourceCode.MemberExpression"/>.
        /// </summary>
        /// <param name="summary">The summary documentation of the <see cref="SourceCode.MemberExpression"/>.</param>
        void SetSummary(string summary);

        /// <summary>
        /// Set the summary documentation of the <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MemberExpression"/>.
        /// </param>
        void SetSummary(CommentExpression summary);

        /// <summary>
        /// Gives the <see cref="MemberExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="MemberExpression"/>.
        /// </param>
        void SetVisibility(MemberVisibility visibility);
    }
}