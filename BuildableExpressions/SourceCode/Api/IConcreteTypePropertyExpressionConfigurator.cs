namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> for a
    /// <see cref="ClassExpression"/> or <see cref="StructExpression"/>.
    /// </summary>
    public interface IConcreteTypePropertyExpressionConfigurator : IPropertyExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="PropertyExpression"/> as static.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="PropertyExpression"/> has already been marked as abstract.
        /// </exception>
        void SetStatic();

        /// <summary>
        /// Add a getter to the <see cref="PropertyExpression"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IPropertyGetterConfigurator"/> to use.</param>
        void SetGetter(Action<IPropertyGetterConfigurator> configuration);

        /// <summary>
        /// Add a setter to the <see cref="PropertyExpression"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IPropertySetterConfigurator"/> to use.</param>
        void SetSetter(Action<IPropertySetterConfigurator> configuration);
    }
}