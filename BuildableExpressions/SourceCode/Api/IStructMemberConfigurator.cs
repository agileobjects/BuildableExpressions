namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/> for a
    /// <see cref="StructExpression"/>.
    /// </summary>
    public interface IStructMemberConfigurator
    {
        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="StructExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(
            string name,
            Action<IConcreteTypeMethodExpressionConfigurator> configuration);
    }
}