namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/> for a
    /// <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassMethodExpressionConfigurator :
        IConcreteTypeMethodExpressionConfigurator,
        IClassMemberExpressionConfigurator
    {
        /// <summary>
        /// Mark the class <see cref="MethodExpression"/> as abstract, with the given
        /// <paramref name="returnType"/>.
        /// </summary>
        /// <param name="returnType">
        /// The return <see cref="IType"/> to apply to the <see cref="MethodExpression"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the class
        /// <see cref="MethodExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MethodExpression"/> has already been marked as static.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MethodExpression"/> has already been marked as virtual.
        /// </exception>
        void SetAbstract(IType returnType);

        /// <summary>
        /// Mark the <see cref="MethodExpression"/> as an extension method. The first added
        /// parameter added will be the subject of the extension.
        /// </summary>
        void SetExtensionMethod();
    }
}