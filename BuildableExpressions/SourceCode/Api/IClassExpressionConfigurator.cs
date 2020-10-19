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
    }
}