namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to add a <see cref="ConstructorExpression"/> to a
    /// <see cref="ConcreteTypeExpression"/>.
    /// </summary>
    public interface IConstructorConfigurator
    {
        /// <summary>
        /// Add a <see cref="ConstructorExpression"/> to the <see cref="ConcreteTypeExpression"/>,
        /// with the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="ConstructorExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="ConstructorExpression"/>.</returns>
        ConstructorExpression AddConstructor(
            Action<IConstructorExpressionConfigurator> configuration);
    }
}