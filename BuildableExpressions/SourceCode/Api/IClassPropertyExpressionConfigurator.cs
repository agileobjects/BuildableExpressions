namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> for a
    /// <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassPropertyExpressionConfigurator : IConcreteTypePropertyExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="PropertyExpression"/> as abstract.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the
        /// <see cref="PropertyExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="PropertyExpression"/> has already been marked as static.
        /// </exception>
        void SetAbstract();
    }
}