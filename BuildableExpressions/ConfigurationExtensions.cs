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
        /// Add a public, instance-scoped <see cref="MethodExpression"/> to the <see cref="TypeExpression"/>,
        /// with the given <paramref name="name"/> and <paramref name="body"/>.
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
            return typeConfig.AddMethod(name, cfg => cfg.SetBody(body));
        }

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="TypeExpression"/>, with the
        /// given <paramref name="name"/> and <paramref name="body"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this ITypeExpressionConfigurator typeConfig,
            string name,
            Expression body,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return typeConfig.AddMethod(name, cfg =>
            {
                configuration.Invoke(cfg);
                cfg.SetBody(body);
            });
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
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="parameters">The <see cref="GenericParameterExpression"/>s to add.</param>
        public static void AddGenericParameters(
            this IMethodExpressionConfigurator methodConfig,
            IEnumerable<GenericParameterExpression> parameters)
        {
            methodConfig.AddGenericParameters(parameters.ToArray());
        }

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="parameter">The ParameterExpression to add.</param>
        public static void AddParameter(
            this IMethodExpressionConfigurator methodConfig,
            ParameterExpression parameter)
        {
            methodConfig.AddParameters(parameter);
        }

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="parameters">The ParameterExpressions to add.</param>
        public static void AddParameters(
            this IMethodExpressionConfigurator methodConfig,
            IEnumerable<ParameterExpression> parameters)
        {
            methodConfig.AddParameters(parameters.ToArray());
        }

        /// <summary>
        /// Set the parameters and body of the <see cref="MethodExpression"/> to those of the given
        /// <paramref name="definition" />.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="definition">The LambdaExpression to use.</param>
        public static void SetDefinition(
            this IMethodExpressionConfigurator methodConfig,
            LambdaExpression definition)
        {
            methodConfig.AddParameters(definition.Parameters);
            methodConfig.SetBody(definition.Body, definition.ReturnType);
        }

        /// <summary>
        /// Set the body of the <see cref="MethodExpression"/>, using the body Expression's Type as
        /// the <see cref="MethodExpression"/>'s return type.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="body">The Expression to use.</param>
        public static void SetBody(
            this IMethodExpressionConfigurator methodConfig,
            Expression body)
        {
            methodConfig.SetBody(body, body.Type);
        }
    }
}