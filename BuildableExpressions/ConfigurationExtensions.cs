namespace AgileObjects.BuildableExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using SourceCode;
    using SourceCode.Api;

    /// <summary>
    /// Provides extension methods for easier API use.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Set the summary documentation of the <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="summary">The summary documentation of the <see cref="TypeExpression"/>.</param>
        public static void SetSummary(
            this ITypeExpressionConfigurator typeConfig,
            string summary)
        {
            typeConfig.SetSummary(ReadableExpression.Comment(summary));
        }

        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The type of interface the <see cref="TypeExpression"/> being built should implement.
        /// </typeparam>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        public static void SetImplements<TInterface>(
            this ITypeExpressionConfigurator typeConfig)
            where TInterface : class
        {
            typeConfig.SetImplements(typeof(TInterface));
        }

        /// <summary>
        /// Add a public <see cref="MethodExpression"/> to the <see cref="TypeExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="body"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this ITypeExpressionConfigurator typeConfig,
            string name,
            Expression body)
        {
            return typeConfig.AddMethod(name, body, cfg => { });
        }

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <typeparamref name="T"/> Type.
        /// </summary>
        /// <typeparam name="T">The Type to which to constrain the <see cref="GenericParameterExpression"/>.</typeparam>
        /// <param name="parameterConfig">The <see cref="IGenericParameterExpressionConfigurator"/> to configure.</param>
        public static void AddTypeConstraint<T>(
            this IGenericParameterExpressionConfigurator parameterConfig)
        {
            parameterConfig.AddTypeConstraint(typeof(T));
        }

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="parameterConfig">The <see cref="IGenericParameterExpressionConfigurator"/> to configure.</param>
        /// <param name="type">The Type to which to constrain the <see cref="GenericParameterExpression"/>.</param>
        public static void AddTypeConstraint(
            this IGenericParameterExpressionConfigurator parameterConfig,
            Type type)
        {
            parameterConfig.AddTypeConstraints(type);
        }

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <paramref name="types"/>.
        /// </summary>
        /// <param name="parameterConfig">The <see cref="IGenericParameterExpressionConfigurator"/> to configure.</param>
        /// <param name="types">The Types to which to constrain the <see cref="GenericParameterExpression"/>.</param>
        public static void AddTypeConstraints(
            this IGenericParameterExpressionConfigurator parameterConfig,
            IEnumerable<Type> types)
        {
            parameterConfig.AddTypeConstraints(types.ToArray());
        }

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        public static void AddGenericParameter(
            this IMethodExpressionConfigurator methodConfig,
            GenericParameterExpression parameter)
        {
            methodConfig.AddGenericParameters(parameter);
        }

        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="summary">The summary documentation of the <see cref="MethodExpression"/>.</param>
        public static void SetSummary(
            this IMethodExpressionConfigurator methodConfig,
            string summary)
        {
            methodConfig.SetSummary(ReadableExpression.Comment(summary));
        }

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="parameters">The <see cref="GenericParameterExpression"/> to add.</param>
        public static void AddGenericParameters(
            this IMethodExpressionConfigurator methodConfig,
            IEnumerable<GenericParameterExpression> parameters)
        {
            methodConfig.AddGenericParameters(parameters.ToArray());
        }
    }
}