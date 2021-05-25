namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure the application of an <see cref="AttributeExpression"/> to a
    /// source code element.
    /// </summary>
    public interface IAttributeApplicationConfigurator
    {
        /// <summary>
        /// Sets the given <paramref name="arguments"/> as the values to pass to the constructor of
        /// the <see cref="AttributeExpression"/> being applied.
        /// </summary>
        /// <param name="arguments">
        /// One or more values to pass to the <see cref="AttributeExpression"/>'s constructor.
        /// </param>
        void SetConstructorArguments(params object[] arguments);
    }
}