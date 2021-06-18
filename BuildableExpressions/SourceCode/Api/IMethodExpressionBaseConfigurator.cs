namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpressionBase"/>.
    /// </summary>
    public interface IMethodExpressionBaseConfigurator : IMemberExpressionConfigurator
    {
        /// <summary>
        /// Add a ParameterExpression to the <see cref="MethodExpressionBase"/> with the given
        /// <paramref name="name"/> and <typeparamref name="TParameter"/> type.
        /// </summary>
        /// <typeparam name="TParameter">The type of the ParameterExpression to add.</typeparam>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        /// <returns>The added ParameterExpression.</returns>
        ParameterExpression AddParameter<TParameter>(string name);

        /// <summary>
        /// Add a ParameterExpression to the <see cref="MethodExpressionBase"/> with the given
        /// <paramref name="name"/>, <typeparamref name="TParameter"/> type and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TParameter">The type of the ParameterExpression to add.</typeparam>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The added ParameterExpression.</returns>
        ParameterExpression AddParameter<TParameter>(
            string name,
            Action<IParameterExpressionConfigurator> configuration);

        /// <summary>
        /// Add a ParameterExpression to the <see cref="MethodExpressionBase"/> with the given
        /// <paramref name="name"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the ParameterExpression to add.</param>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        /// <returns>The added ParameterExpression.</returns>
        ParameterExpression AddParameter(Type type, string name);

        /// <summary>
        /// Add a ParameterExpression to the <see cref="MethodExpressionBase"/> with the given
        /// <paramref name="name"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the ParameterExpression to add.</param>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The added ParameterExpression.</returns>
        ParameterExpression AddParameter(
            Type type,
            string name,
            Action<IParameterExpressionConfigurator> configuration);

        /// <summary>
        /// Add the given <paramref name="parameter"/> to the <see cref="MethodExpressionBase"/>.
        /// </summary>
        /// <param name="parameter">The ParameterExpression to add.</param>
        void AddParameter(ParameterExpression parameter);

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
        /// Add the given <paramref name="parameters"/> to the <see cref="MethodExpressionBase"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpressions to add.</param>
        void AddParameters(IEnumerable<ParameterExpression> parameters);

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpressionBase"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpression to add.</param>
        void AddParameters(params ParameterExpression[] parameters);
    }
}