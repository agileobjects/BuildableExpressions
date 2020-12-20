namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="ConstructorExpression"/> for a
    /// <see cref="ClassExpression"/> or <see cref="StructExpression"/>.
    /// </summary>
    public interface IConstructorExpressionConfigurator : IMemberExpressionConfigurator
    {
        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="ConstructorExpression"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpression to add.</param>
        void AddParameters(params ParameterExpression[] parameters);

        /// <summary>
        /// Adds a call from the <see cref="ConstructorExpression"/> to the given sibling or base
        /// Type (if applicable) <paramref name="targetConstructorExpression"/>.
        /// </summary>
        /// <param name="targetConstructorExpression">
        /// The sibling or base Type <see cref="ConstructorExpression"/> to call.
        /// </param>
        /// <param name="arguments">
        /// Zero or more Expressions to pass to the given sibling or base Type
        /// <paramref name="targetConstructorExpression"/>.
        /// </param>
        void SetConstructorCall(
            ConstructorExpression targetConstructorExpression,
            params Expression[] arguments);

        /// <summary>
        /// Set the body of the <see cref="ConstructorExpression"/>.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        void SetBody(Expression body);
    }
}