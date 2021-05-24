namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure an <see cref="AttributeExpression"/>.
    /// </summary>
    public interface IAttributeExpressionConfigurator
    {
        /// <summary>
        /// Configures the <see cref="AttributeExpression"/> to derive from the given
        /// <paramref name="baseAttributeExpression"/>.
        /// </summary>
        /// <param name="baseAttributeExpression">
        /// The base <see cref="AttributeExpression"/> from which the
        /// <see cref="AttributeExpression"/> should derive.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="AttributeExpression"/> has already been given a base type.
        /// </exception>
        void SetBaseType(
            AttributeExpression baseAttributeExpression,
            Action<IAttributeImplementationConfigurator> configuration);
    }
}