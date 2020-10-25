namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure an <see cref="InterfaceExpression"/>.
    /// </summary>
    public interface IInterfaceExpressionConfigurator : ITypeExpressionConfigurator
    {
        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="InterfaceExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="returnType">The return type of the <see cref="MethodExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(
            string name,
            Type returnType,
            Action<IMethodExpressionConfigurator> configuration);
    }
}