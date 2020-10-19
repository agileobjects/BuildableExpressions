namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="TypeExpression"/>.
    /// </summary>
    public interface ITypeExpressionConfigurator
    {
        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>. If a single configured Method matches a single
        /// interface method declaration, it will be named after that interface method.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The type of interface the <see cref="TypeExpression"/> being built should implement.
        /// </typeparam>
        void SetImplements<TInterface>() where TInterface : class;

        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the given
        /// <paramref name="interfaces"/>. If a single configured Method matches a single
        /// interface method declaration, it will be named after that interface method.
        /// </summary>
        /// <param name="interfaces">
        /// The type of interfaces the <see cref="TypeExpression"/> being built should implement.
        /// </param>
        void SetImplements(params Type[] interfaces);

        /// <summary>
        /// Set the summary documentation of the <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="summary">The summary documentation of the <see cref="TypeExpression"/>.</param>
        void SetSummary(string summary);

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

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="TypeExpression"/>, with an
        /// auto-generated name and the given <paramref name="body"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(string name, Expression body);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="TypeExpression"/>, with an
        /// auto-generated name and the given <paramref name="body"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(
            string name,
            Expression body,
            Action<IMethodExpressionConfigurator> configuration);
    }
}