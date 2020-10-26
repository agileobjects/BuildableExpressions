namespace AgileObjects.BuildableExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
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
        /// Closes the given <paramref name="parameter"/> to the given <typeparamref name="TClosed"/>
        /// for the <see cref="TypeExpression"/>
        /// </summary>
        /// <typeparam name="TClosed">
        /// The Type to which to close the given <paramref name="parameter"/>.
        /// </typeparam>
        /// <param name="implConfig">The <see cref="IImplementationConfigurator"/> to configure.</param>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to
        /// close to the given <typeparamref name="TClosed"/>.
        /// </param>
        public static void SetGenericArgument<TClosed>(
            this IImplementationConfigurator implConfig,
            GenericParameterExpression parameter)
        {
            implConfig.SetGenericArgument(parameter, typeof(TClosed));
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseTypeExpression"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="baseTypeExpression">
        /// The base type from which the <see cref="ClassExpression"/> being built should derive.
        /// </param>
        public static void SetBaseType(
            this IClassExpressionConfigurator classConfig,
            ClassExpression baseTypeExpression)
        {
            classConfig.SetBaseType(baseTypeExpression.Type);
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
        /// Configures the <see cref="TypeExpression"/> to implement the Types of the given
        /// <paramref name="interfaceExpressions"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="interfaceExpressions">
        /// One or more <see cref="InterfaceExpression"/>s the <see cref="TypeExpression"/> should
        /// implement.
        /// </param>
        public static void SetImplements(
            this ITypeExpressionConfigurator typeConfig,
            params InterfaceExpression[] interfaceExpressions)
        {
            typeConfig.SetImplements(interfaceExpressions.ProjectToArray(ie => ie.Type));
        }

        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the Types of the given
        /// <paramref name="interfaceExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="interfaceExpression">
        /// The <see cref="InterfaceExpression"/>s the <see cref="TypeExpression"/> should implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements(
            this ITypeExpressionConfigurator typeConfig,
            InterfaceExpression interfaceExpression,
            Action<IImplementationConfigurator> configuration)
        {
            typeConfig.SetImplements(interfaceExpression.Type, configuration);
        }

        /// <summary>
        /// Add a parameterless <see cref="MethodExpression"/> to the
        /// <see cref="InterfaceExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="returnType"/>.
        /// </summary>
        /// <param name="interfaceConfig">The <see cref="IInterfaceExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="returnType">The return type of the <see cref="MethodExpression"/>.</param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IInterfaceExpressionConfigurator interfaceConfig,
            string name,
            Type returnType)
        {
            return interfaceConfig.AddMethod(name, returnType, cfg => { });
        }

        /// <summary>
        /// Add a public, instance-scoped <see cref="MethodExpression"/> to the
        /// <see cref="ClassExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="body"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IClassExpressionConfigurator classConfig,
            string name,
            Expression body)
        {
            return classConfig.AddMethod(name, cfg => cfg.SetBody(body));
        }

        /// <summary>
        /// Add a public, instance-scoped <see cref="MethodExpression"/> to the
        /// <see cref="StructExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="body"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IStructExpressionConfigurator typeConfig,
            string name,
            Expression body)
        {
            return typeConfig.AddMethod(name, cfg => cfg.SetBody(body));
        }

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="StructExpression"/>, with the
        /// given <paramref name="name"/> and <paramref name="body"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
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
            this IStructExpressionConfigurator typeConfig,
            string name,
            Expression body,
            Action<IConcreteTypeMethodExpressionConfigurator> configuration)
        {
            return typeConfig.AddMethod(name, cfg =>
            {
                configuration.Invoke(cfg);
                cfg.SetBody(body);
            });
        }

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/>, with the
        /// given <paramref name="name"/> and <paramref name="body"/>, using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
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
            this IClassExpressionConfigurator classConfig,
            string name,
            Expression body,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return classConfig.AddMethod(name, cfg =>
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
        /// Add an unconstrained <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="name"/> to the <see cref="TypeExpression"/> or
        /// <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IGenericParameterConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>The newly-created <see cref="GenericParameterExpression"/>.</returns>
        public static GenericParameterExpression AddGenericParameter(
            this IGenericParameterConfigurator methodConfig,
            string name)
        {
            return methodConfig.AddGenericParameter(name, gp => { });
        }

        /// <summary>
        /// Add the given <paramref name="parameter"/> to the <see cref="MethodExpression"/>.
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
        /// Add the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
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
        /// <param name="methodConfig">The <see cref="IConcreteTypeMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="definition">The LambdaExpression to use.</param>
        public static void SetDefinition(
            this IConcreteTypeMethodExpressionConfigurator methodConfig,
            LambdaExpression definition)
        {
            methodConfig.AddParameters(definition.Parameters);
            methodConfig.SetBody(definition.Body, definition.ReturnType);
        }

        /// <summary>
        /// Set the body of the <see cref="MethodExpression"/>, using the body Expression's Type as
        /// the <see cref="MethodExpression"/>'s return type.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IConcreteTypeMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="body">The Expression to use.</param>
        public static void SetBody(
            this IConcreteTypeMethodExpressionConfigurator methodConfig,
            Expression body)
        {
            methodConfig.SetBody(body, body.Type);
        }
    }
}