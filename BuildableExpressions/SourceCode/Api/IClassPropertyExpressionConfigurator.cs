namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyOrFieldExpression"/> for a
    /// <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassPropertyExpressionConfigurator : IConcreteTypePropertyExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="PropertyOrFieldExpression"/> as abstract.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the
        /// <see cref="PropertyOrFieldExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="PropertyOrFieldExpression"/> has already been marked as static.
        /// </exception>
        void SetAbstract();
    }
}