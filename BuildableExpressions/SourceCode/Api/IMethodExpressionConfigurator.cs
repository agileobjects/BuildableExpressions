namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpressionBase"/>.
    /// </summary>
    public interface IMethodExpressionConfigurator :
        IMemberExpressionConfigurator,
        IGenericParameterConfigurator
    {
        /// <summary>
        /// Adds the given <paramref name="parameter"/> to the <see cref="MethodExpressionBase"/>,
        /// using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="parameter">The ParameterExpression to add.</param>
        /// <param name="configuration">The configuration to use.</param>
        void AddParameter(
            ParameterExpression parameter, 
            Action<IParameterExpressionConfigurator> configuration);

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpression to add.</param>
        void AddParameters(params ParameterExpression[] parameters);
    }
}