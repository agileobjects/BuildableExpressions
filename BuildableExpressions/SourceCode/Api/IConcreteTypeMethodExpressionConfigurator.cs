namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/> for a
    /// <see cref="ClassExpression"/> or <see cref="StructExpression"/>.
    /// </summary>
    public interface IConcreteTypeMethodExpressionConfigurator : IMethodExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="MethodExpression"/> as static.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpression"/> has already been marked as abstract.
        /// </exception>
        void SetStatic();

        /// <summary>
        /// Set the body of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        /// <param name="returnType">The return type to use for the method.</param>
        void SetBody(Expression body, Type returnType);
    }
}