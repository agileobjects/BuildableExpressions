namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="StructExpression"/>.
    /// </summary>
    public interface IStructExpressionConfigurator :
        IConcreteTypeExpressionConfigurator,
        IStructMethodConfigurator
    {
        /// <summary>
        /// Configures the <see cref="StructExpression"/> to implement the given
        /// <paramref name="interface"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interface">
        /// The interface type the <see cref="StructExpression"/> should implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        void SetImplements(
            Type @interface,
            Action<IStructImplementationConfigurator> configuration);
    }
}