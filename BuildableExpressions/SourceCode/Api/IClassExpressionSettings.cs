namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions;

    /// <summary>
    /// Provides configuration options to control aspects of <see cref="ClassExpression"/> creation.
    /// </summary>
    public interface IClassExpressionSettings
    {
        /// <summary>
        /// Configures the <see cref="ClassExpression"/> being built to implement the given
        /// <typeparamref name="TInterface"/>. If a single configured Method matches a single
        /// interface method declaration, it will be named after that interface method.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The type of interface the <see cref="ClassExpression"/> being built should implement.
        /// </typeparam>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings Implementing<TInterface>() where TInterface : class;

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> being built to implement the given
        /// <paramref name="interfaces"/>. If a single configured Method matches a single
        /// interface method declaration, it will be named after that interface method.
        /// </summary>
        /// <param name="interfaces">
        /// The type of interfaces the <see cref="ClassExpression"/> being built should implement.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings Implementing(params Type[] interfaces);

        /// <summary>
        /// Set the visibility of the <see cref="ClassExpression"/> being built to the given
        /// <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="ClassVisibility"/> to use for the <see cref="ClassExpression"/> being
        /// built.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithVisibility(ClassVisibility visibility);

        /// <summary>
        /// Set the summary documentation of the <see cref="ClassExpression"/> being built.
        /// </summary>
        /// <param name="summary">The summary documentation of the <see cref="ClassExpression"/> being built.</param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithSummary(string summary);

        /// <summary>
        /// Set the summary documentation of the <see cref="ClassExpression"/> being built.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="ClassExpression"/> being built.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithSummary(CommentExpression summary);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/> being built,
        /// with an auto-generated name and the given <paramref name="body"/>.
        /// </summary>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithMethod(Expression body);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/> being built,
        /// with an auto-generated name and the given <paramref name="body"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <param name="configuration">
        /// The configuration with which to generate the <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithMethod(
            Expression body,
            Func<IMethodExpressionSettings, IMethodExpressionSettings> configuration);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/> being built,
        /// with the given <paramref name="name"/> and <paramref name="body"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/> to create.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithMethod(string name, Expression body);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/> being built,
        /// with the given <paramref name="name"/>, <paramref name="body"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/> to create.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <param name="configuration">
        /// The configuration with which to generate the <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>These <see cref="IClassExpressionSettings"/>, to support a fluent interface.</returns>
        IClassExpressionSettings WithMethod(
            string name,
            Expression body,
            Func<IMethodExpressionSettings, IMethodExpressionSettings> configuration);
    }
}