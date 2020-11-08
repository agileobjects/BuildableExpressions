namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/>.
    /// </summary>
    public interface IMethodExpressionConfigurator :
        IMemberExpressionConfigurator,
        IGenericParameterConfigurator
    {
        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpression to add.</param>
        void AddParameters(params ParameterExpression[] parameters);
    }
}