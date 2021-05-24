namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure source code Expressions to which an
    /// <see cref="AttributeExpression"/> can be applied.
    /// </summary>
    public interface IAttributableExpressionConfigurator
    {
        /// <summary>
        /// Applies the given <paramref name="attribute"/> to the source code element being
        /// configured.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="AttributeExpression"/> to apply to the source code element being
        /// configured.
        /// </param>
        void AddAttribute(AttributeExpression attribute);
    }
}