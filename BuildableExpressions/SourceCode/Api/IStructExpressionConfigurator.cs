namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="StructExpression"/>.
    /// </summary>
    public interface IStructExpressionConfigurator :
        IConcreteTypeExpressionConfigurator,
        IStructConstructorConfigurator,
        IStructMemberConfigurator
    {
        /// <summary>
        /// Configures the <see cref="StructExpression"/> to implement the given
        /// <paramref name="interfaceExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interfaceExpression">The <see cref="InterfaceExpression"/> to implement.</param>
        /// <param name="configuration">The configuration to use.</param>
        void SetImplements(
            InterfaceExpression interfaceExpression,
            Action<IStructImplementationConfigurator> configuration);
    }
}