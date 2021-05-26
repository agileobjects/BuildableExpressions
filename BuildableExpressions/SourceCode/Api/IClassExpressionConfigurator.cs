namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassExpressionConfigurator :
        IConcreteTypeExpressionConfigurator,
        IClassBaseExpressionConfigurator,
        IClassMemberConfigurator
    {
        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseTypeExpression"/>.
        /// </summary>
        /// <param name="baseTypeExpression">
        /// The base <see cref="ClassExpression"/> from which the <see cref="ClassExpression"/>
        /// should derive.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> has already been given a base type.
        /// </exception>
        void SetBaseType(
            ClassExpression baseTypeExpression,
            Action<IClassImplementationConfigurator> configuration);

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to implement the given
        /// <paramref name="interfaceExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interfaceExpression">
        /// The <see cref="InterfaceExpression"/> type the <see cref="ClassExpression"/> should
        /// implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        void SetImplements(
            InterfaceExpression interfaceExpression,
            Action<IClassImplementationConfigurator> configuration);

        /// <summary>
        /// Mark the <see cref="ClassExpression"/> as static. Added methods will be made static
        /// automatically.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> has already been made abstract or sealed.
        /// </exception>
        void SetStatic();
    }
}