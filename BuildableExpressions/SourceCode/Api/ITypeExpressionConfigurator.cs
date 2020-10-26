namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/>.
    /// </summary>
    public interface ITypeExpressionConfigurator : IGenericParameterConfigurator
    {
        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the given
        /// <paramref name="interfaces"/>.
        /// </summary>
        /// <param name="interfaces">
        /// The interface types the <see cref="TypeExpression"/> should implement.
        /// </param>
        void SetImplements(params Type[] interfaces);

        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the given
        /// <paramref name="interface"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interface">
        /// The interface type the <see cref="TypeExpression"/> should implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        void SetImplements(
            Type @interface,
            Action<IImplementationConfigurator> configuration);

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