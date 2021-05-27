namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure an <see cref="AttributeExpression"/>.
    /// </summary>
    public interface IAttributeExpressionConfigurator : IClassBaseExpressionConfigurator
    {
        /// <summary>
        /// Set the System.AttributeTargets restricting to which code elements the
        /// <see cref="AttributeExpression"/> can be applied, if restricted usage is required.
        /// </summary>
        /// <param name="targets">
        /// The System.AttributeTargets restricting the set of code elements to which the
        /// <see cref="AttributeExpression"/> can be applied.
        /// </param>
        void SetValidOn(AttributeTargets targets);

        /// <summary>
        /// Allow the <see cref="AttributeExpression"/> to be applied to the same code element
        /// multiple times.
        /// </summary>
        void SetMultipleAllowed();

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