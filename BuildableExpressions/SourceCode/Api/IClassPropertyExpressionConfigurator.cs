namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> for a
    /// <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassPropertyExpressionConfigurator :
        IConcreteTypePropertyExpressionConfigurator,
        IClassMemberExpressionConfigurator
    {
        /// <summary>
        /// Mark the class <see cref="PropertyExpression"/> as abstract.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the class
        /// <see cref="PropertyExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="PropertyExpression"/> has already been marked as static.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="PropertyExpression"/> has already been marked as virtual.
        /// </exception>
        void SetAbstract();
    }
}