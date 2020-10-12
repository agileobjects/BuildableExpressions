namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassExpressionConfigurator
    {
        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>. If a single configured Method matches a single
        /// interface method declaration, it will be named after that interface method.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The type of interface the <see cref="ClassExpression"/> being built should implement.
        /// </typeparam>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator Implementing<TInterface>() where TInterface : class;

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to implement the given
        /// <paramref name="interfaces"/>. If a single configured Method matches a single
        /// interface method declaration, it will be named after that interface method.
        /// </summary>
        /// <param name="interfaces">
        /// The type of interfaces the <see cref="ClassExpression"/> being built should implement.
        /// </param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator Implementing(params Type[] interfaces);

        /// <summary>
        /// Set the summary documentation of the <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="summary">The summary documentation of the <see cref="ClassExpression"/>.</param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator WithSummary(string summary);

        /// <summary>
        /// Set the summary documentation of the <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="ClassExpression"/>.
        /// </param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator WithSummary(CommentExpression summary);

        /// <summary>
        /// Set the visibility of the <see cref="ClassExpression"/> to the given
        /// <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="ClassVisibility"/> to use for the <see cref="ClassExpression"/>.
        /// </param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator WithVisibility(ClassVisibility visibility);

        /// <summary>
        /// Mark the <see cref="ClassExpression"/> as static. Added methods will be made static
        /// automatically.
        /// </summary>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator AsStatic();

        /// <summary>
        /// Set the name of the <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ClassExpression"/>.</param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator Named(string name);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/>, with an
        /// auto-generated name and the given <paramref name="body"/>.
        /// </summary>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator WithMethod(Expression body);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/>, with an
        /// auto-generated name and the given <paramref name="body"/>.
        /// </summary>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>
        /// .</param>
        /// <returns>This <see cref="IClassExpressionConfigurator"/>, to support a fluent API.</returns>
        IClassExpressionConfigurator WithMethod(
            Expression body,
            Func<IMethodExpressionConfigurator, IMethodExpressionConfigurator> configuration);
    }
}