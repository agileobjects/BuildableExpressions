namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="ClassExpressionBase"/>.
    /// </summary>
    public interface IClassBaseExpressionConfigurator : 
        IConcreteTypeExpressionConfigurator,
        IClassMemberConfigurator
    {
        /// <summary>
        /// Gets an Expression to use to refer to the base class instance of the type being created
        /// in the current scope. Use this property to access the 'base' keyword in a class method.
        /// </summary>
        Expression BaseInstanceExpression { get; }

        /// <summary>
        /// Mark the <see cref="ClassExpressionBase"/> as abstract.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpressionBase"/> has already been made static or sealed.
        /// </exception>
        void SetAbstract();

        /// <summary>
        /// Mark the <see cref="ClassExpressionBase"/> as sealed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpressionBase"/> has already been made static or abstract.
        /// </exception>
        void SetSealed();
    }
}