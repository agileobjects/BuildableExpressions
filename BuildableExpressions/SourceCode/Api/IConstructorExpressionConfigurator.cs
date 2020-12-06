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
        /// Set the body of the <see cref="ConstructorExpression"/>.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        void SetBody(Expression body);
    }
}