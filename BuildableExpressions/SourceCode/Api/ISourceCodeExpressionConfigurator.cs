namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="SourceCodeExpression"/>.
    /// </summary>
    public interface ISourceCodeExpressionConfigurator
    {
        /// <summary>
        /// Sets the header documentation of the <see cref="SourceCodeExpression"/> to the given
        /// <paramref name="header"/>.
        /// </summary>
        /// <param name="header">
        /// A <see cref="CommentExpression"/> to use for the file header of the source code generated
        /// from the <see cref="SourceCodeExpression"/>.
        /// </param>
        void SetHeader(CommentExpression header);

        /// <summary>
        /// Add <see cref="TypeExpression"/>s belonging to the <see cref="SourceCodeExpression"/>
        /// to the given <paramref name="namespace"/>.
        /// </summary>
        /// <param name="namespace">The namespace to which generated classes should belong.</param>
        void SetNamespace(string @namespace);

        /// <summary>
        /// Adds a new <see cref="InterfaceExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="InterfaceExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="InterfaceExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="InterfaceExpression"/>.</returns>
        InterfaceExpression AddInterface(string name, Action<IInterfaceExpressionConfigurator> configuration);

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

        /// <summary>
        /// Adds a new <see cref="AttributeExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="AttributeExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="AttributeExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="AttributeExpression"/>.</returns>
        AttributeExpression AddAttribute(string name, Action<IAttributeExpressionConfigurator> configuration);

        /// <summary>
        /// Adds a new <see cref="EnumExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="EnumExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="EnumExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="EnumExpression"/>.</returns>
        EnumExpression AddEnum(string name, Action<IEnumExpressionConfigurator> configuration);
    }
}