namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/> for a
    /// <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassMethodExpressionConfigurator : IConcreteTypeMethodExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="MethodExpression"/> as abstract.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the
        /// <see cref="MethodExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpression"/> has already been marked as static.
        /// </exception>
        void SetAbstract();
    }
}