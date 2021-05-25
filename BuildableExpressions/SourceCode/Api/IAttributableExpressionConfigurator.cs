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