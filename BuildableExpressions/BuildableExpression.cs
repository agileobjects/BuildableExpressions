namespace AgileObjects.BuildableExpressions
{
    using System;
    using SourceCode;
    using SourceCode.Api;

    /// <summary>
    /// Provides buildable Expression factory methods.
    /// </summary>
    public static partial class BuildableExpression
    {
        /// <summary>
        /// Create a <see cref="SourceCodeExpression"/> representing a complete piece of source code.
        /// </summary>
        /// <param name="configuration">The configuration to use for the <see cref="SourceCodeExpression"/>.</param>
        /// <returns>A <see cref="SourceCodeExpression"/> representing a complete piece of source code.</returns>
        public static SourceCodeExpression SourceCode(
            Action<ISourceCodeExpressionConfigurator> configuration)
        {
            return new SourceCodeExpression(configuration);
        }
    }
}
