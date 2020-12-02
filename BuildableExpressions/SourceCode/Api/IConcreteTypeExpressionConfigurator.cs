namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Linq.Expressions;
    using Generics;

    /// <summary>
    /// Provides options to configure a <see cref="ConcreteTypeExpression"/>.
    /// </summary>
    public interface IConcreteTypeExpressionConfigurator : ITypeableTypeExpressionConfigurator
    {
        /// <summary>
        /// Gets an Expression to use to refer to the instance of the type being created in the
        /// current scope. Use this property to access the 'this' keyword in a class or struct method.
        /// </summary>
        Expression ThisInstanceExpression { get; }

        /// <summary>
        /// Adds a copy of the given open generic <paramref name="parameter"/> to the
        /// <see cref="ConcreteTypeExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        /// <returns>The newly-created copy of the given <paramref name="parameter"/>.</returns>
        GenericParameterExpression AddGenericParameter(GenericParameterExpression parameter);
    }
}