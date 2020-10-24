namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassExpressionConfigurator : IConcreteTypeExpressionConfigurator
    {
        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">
        /// The base type from which the <see cref="ClassExpression"/> being built should derive.
        /// </typeparam>
        void SetBaseType<TBase>() where TBase : class;

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">
        /// The base type from which the <see cref="ClassExpression"/> being built should derive.
        /// </param>
        void SetBaseType(Type baseType);

        /// <summary>
        /// Mark the <see cref="ClassExpression"/> as static. Added methods will be made static
        /// automatically.
        /// </summary>
        void SetStatic();

        /// <summary>
        /// Mark the <see cref="ClassExpression"/> as abstract.
        /// </summary>
        void SetAbstract();

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration);
    }
}