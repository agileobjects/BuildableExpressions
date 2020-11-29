namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Provides options to configure an <see cref="InterfaceExpression"/>.
    /// </summary>
    public interface IInterfaceExpressionConfigurator : ITypeableTypeExpressionConfigurator
    {
        /// <summary>
        /// Configures the <see cref="InterfaceExpression"/> to implement the given
        /// <paramref name="interfaceExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interfaceExpression">
        /// The interface type the <see cref="InterfaceExpression"/> should implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        void SetImplements(
            InterfaceExpression interfaceExpression,
            Action<IImplementationConfigurator> configuration);

        /// <summary>
        /// Add a <see cref="PropertyExpression"/> to the <see cref="InterfaceExpression"/>, with
        /// the given <paramref name="name"/>, <paramref name="type"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The <see cref="IType"/> of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        PropertyExpression AddProperty(
            string name,
            IType type,
            Action<IInterfacePropertyExpressionConfigurator> configuration);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="InterfaceExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="returnType">The return <see cref="IType"/> of the <see cref="MethodExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(
            string name,
            IType returnType,
            Action<IMethodExpressionConfigurator> configuration);
    }
}