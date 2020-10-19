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
        void SetNamespace(string @namespace);

        /// <summary>
        /// Adds a new <see cref="ClassExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ClassExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="ClassExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="ClassExpression"/>.</returns>
        ClassExpression AddClass(string name, Action<IClassExpressionConfigurator> configuration);

        /// <summary>
        /// Adds a new <see cref="StructExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="StructExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="StructExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="StructExpression"/>.</returns>
        StructExpression AddStruct(string name, Action<IStructExpressionConfigurator> configuration);
    }
}