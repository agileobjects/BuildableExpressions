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
        /// Create a <see cref="ThisInstanceExpression"/> that represents the instance of an object
        /// to which the 'this' keyword relates in the current context.
        /// </summary>
        /// <param name="class">The <see cref="ClassExpression"/> representing the instance Type.</param>
        /// <returns>The <see cref="ThisInstanceExpression"/> representing the object instance.</returns>
        public static ThisInstanceExpression ThisInstance(ClassExpression @class)
            => new ThisInstanceExpression(@class);

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
