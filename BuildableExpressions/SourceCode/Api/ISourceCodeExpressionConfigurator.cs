namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="SourceCodeExpression"/>.
    /// </summary>
    public interface ISourceCodeExpressionConfigurator
    {
        /// <summary>
        /// Add generated classes to the namespace of the given <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type the namespace to which generated code should belong.</typeparam>
        /// <returns>This <see cref="ISourceCodeExpressionConfigurator"/>, to support a fluent API.</returns>
        ISourceCodeExpressionConfigurator WithNamespaceOf<T>();

        /// <summary>
        /// Add generated classes to the namespace of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type the namespace of which the generated code should use.</param>
        /// <returns>This <see cref="ISourceCodeExpressionConfigurator"/>, to support a fluent API.</returns>
        ISourceCodeExpressionConfigurator WithNamespaceOf(Type type);

        /// <summary>
        /// Add generated classes to the given <paramref name="namespace"/>.
        /// </summary>
        /// <param name="namespace">The namespace to which generated classes should belong.</param>
        /// <returns>This <see cref="ISourceCodeExpressionConfigurator"/>, to support a fluent API.</returns>
        ISourceCodeExpressionConfigurator WithNamespace(string @namespace);

        /// <summary>
        /// Add a <see cref="ClassExpression"/> to the <see cref="SourceCodeExpression"/> using the
        /// given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">
        /// The configuration with which to generate the <see cref="ClassExpression"/>.
        /// </param>
        /// <returns>This <see cref="ISourceCodeExpressionConfigurator"/>, to support a fluent API.</returns>
        ISourceCodeExpressionConfigurator WithClass(
            Func<IClassExpressionConfigurator, IClassExpressionConfigurator> configuration);
    }
}