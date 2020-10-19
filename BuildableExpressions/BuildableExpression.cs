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

        /// <summary>
        /// Create a <see cref="GenericParameterExpression"/> that represents a class or method
        /// open generic argument, with the given <paramref name="name"/> and no type contraints.
        /// </summary>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>
        /// The <see cref="GenericParameterExpression"/> representing a class or method open generic
        /// argument.
        /// </returns>
        public static GenericParameterExpression GenericParameter(string name)
            => GenericParameter(name, gp => gp);

        /// <summary>
        /// Create a <see cref="GenericParameterExpression"/> that represents a class or method
        /// open generic argument, with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <param name="configuration">The configuration to use for the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>
        /// The <see cref="GenericParameterExpression"/> representing a class or method open generic
        /// argument.
        /// </returns>
        public static GenericParameterExpression GenericParameter(
            string name,
            Func<IGenericParameterExpressionConfigurator, IGenericParameterExpressionConfigurator> configuration)
        {
            return new GenericParameterExpression(name, configuration);
        }
    }
}
