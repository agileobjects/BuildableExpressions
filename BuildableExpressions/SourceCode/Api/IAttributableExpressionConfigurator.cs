namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure source code Expressions to which an
    /// <see cref="AttributeExpression"/> can be applied.
    /// </summary>
    public interface IAttributableExpressionConfigurator
    {
        /// <summary>
        /// Applies the <typeparamref name="TAttribute"/> type to the source code element being
        /// configured.
        /// </summary>
        /// <typeparam name="TAttribute">
        /// The Attribute type to apply to the source code element being configured.
        /// </typeparam>
        void AddAttribute<TAttribute>();

        /// <summary>
        /// Applies the <typeparamref name="TAttribute"/> type to the source code element being
        /// configured, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TAttribute">
        /// The Attribute type to apply to the source code element being configured.
        /// </typeparam>
        /// <param name="configuration">The configuration to use.</param>
        void AddAttribute<TAttribute>(Action<IAttributeApplicationConfigurator> configuration);

        /// <summary>
        /// Applies the <paramref name="attributeType"/> attribute to the source code element being
        /// configured.
        /// </summary>
        /// <param name="attributeType">
        /// The Attribute type to apply to the source code element being configured.
        /// </param>
        void AddAttribute(Type attributeType);

        /// <summary>
        /// Applies the <paramref name="attributeType"/> attribute to the source code element being
        /// configured, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="attributeType">
        /// The Attribute type to apply to the source code element being configured.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        void AddAttribute(
            Type attributeType,
            Action<IAttributeApplicationConfigurator> configuration);

        /// <summary>
        /// Applies the given <paramref name="attribute"/> to the source code element being
        /// configured.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="AttributeExpression"/> to apply to the source code element being
        /// configured.
        /// </param>
        void AddAttribute(AttributeExpression attribute);

        /// <summary>
        /// Applies the given <paramref name="attribute"/> to the source code element being
        /// configured, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="AttributeExpression"/> to apply to the source code element being
        /// configured.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>
        /// The <see cref="AppliedAttribute"/> describing the application of the given
        /// <paramref name="attribute"/>.
        /// </returns>
        AppliedAttribute AddAttribute(
            AttributeExpression attribute,
            Action<IAttributeApplicationConfigurator> configuration);
    }
}