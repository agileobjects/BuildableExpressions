namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> or
    /// <see cref="MethodExpression"/> for a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassMemberExpressionConfigurator
    {
        /// <summary>
        /// Mark the class <see cref="MemberExpression"/> as virtual.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MemberExpression"/> has already been marked as static.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MemberExpression"/> has already been marked as abstract.
        /// </exception>
        void SetVirtual();
    }
}