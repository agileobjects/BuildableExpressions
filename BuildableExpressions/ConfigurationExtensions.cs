namespace AgileObjects.BuildableExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;
    using SourceCode;
    using SourceCode.Api;
    using SourceCode.Extensions;
    using SourceCode.Generics;
    using static SourceCode.MemberVisibility;

    /// <summary>
    /// Provides extension methods for easier API use.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds a public <see cref="EnumExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="sourceCodeConfig">The <see cref="ISourceCodeExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="EnumExpression"/>.</param>
        /// <param name="memberNames">The names of the members of the new <see cref="EnumExpression"/>.</param>
        /// <returns>The newly-created <see cref="EnumExpression"/>.</returns>
        public static EnumExpression AddEnum(
            this ISourceCodeExpressionConfigurator sourceCodeConfig,
            string name,
            params string[] memberNames)
        {
            return sourceCodeConfig.AddEnum(name, enm =>
            {
                enm.AddMembers(memberNames);
            });
        }

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
        /// Closes the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/> to the given <typeparamref name="TClosed"/> type
        /// for the <see cref="TypeExpression"/>
        /// </summary>
        /// <typeparam name="TClosed">
        /// The Type to which to close the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/>.
        /// </typeparam>
        /// <param name="implConfig">The <see cref="IImplementationConfigurator"/> to configure.</param>
        /// <param name="genericParameterName">
        /// The name of the <see cref="GenericParameterExpression"/> describing the open generic
        /// parameter to close to the given <typeparamref name="TClosed"/> type.
        /// </param>
        public static void SetGenericArgument<TClosed>(
            this IImplementationConfigurator implConfig,
            string genericParameterName)
        {
            implConfig.SetGenericArgument(genericParameterName, typeof(TClosed));
        }

        /// <summary>
        /// Closes the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/> to the given <paramref name="closedType"/> type
        /// for the <see cref="TypeExpression"/>
        /// </summary>
        /// <param name="implConfig">The <see cref="IImplementationConfigurator"/> to configure.</param>
        /// <param name="genericParameterName">
        /// The name of the <see cref="GenericParameterExpression"/> describing the open generic
        /// parameter to close to the given <paramref name="closedType"/> type.
        /// </param>
        /// <param name="closedType">
        /// The Type to which to close the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/>.
        /// </param>
        public static void SetGenericArgument(
            this IImplementationConfigurator implConfig,
            string genericParameterName,
            Type closedType)
        {
            implConfig.SetGenericArgument(
                genericParameterName,
                TypeExpressionFactory.Create(closedType));
        }

        /// <summary>
        /// Closes the given <paramref name="parameter"/> to the given <typeparamref name="TClosed"/>
        /// type for the <see cref="TypeExpression"/>
        /// </summary>
        /// <typeparam name="TClosed">
        /// The Type to which to close the given <paramref name="parameter"/>.
        /// </typeparam>
        /// <param name="implConfig">The <see cref="IImplementationConfigurator"/> to configure.</param>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to
        /// close to the given <typeparamref name="TClosed"/> type.
        /// </param>
        public static void SetGenericArgument<TClosed>(
            this IImplementationConfigurator implConfig,
            GenericParameterExpression parameter)
        {
            implConfig.SetGenericArgument(parameter, typeof(TClosed));
        }

        /// <summary>
        /// Closes the given <paramref name="parameter"/> to the given <paramref name="closedType"/>
        /// type for the <see cref="TypeExpression"/>
        /// </summary>
        /// <param name="implConfig">The <see cref="IImplementationConfigurator"/> to configure.</param>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to 
        /// close to the given <paramref name="closedType"/>.
        /// </param>
        /// <param name="closedType">The Type to which to close the given <paramref name="parameter"/>.</param>
        public static void SetGenericArgument(
            this IImplementationConfigurator implConfig,
            GenericParameterExpression parameter,
            Type closedType)
        {
            implConfig.SetGenericArgument(parameter, TypeExpressionFactory.Create(closedType));
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <typeparamref name="TBase"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <typeparam name="TBase">
        /// The base type from which the <see cref="ClassExpression"/> should derive.
        /// </typeparam>
        public static void SetBaseType<TBase>(
            this IClassExpressionConfigurator classConfig)
            where TBase : class
        {
            classConfig.SetBaseType(typeof(TBase));
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseType"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="baseType">
        /// The base Type from which the <see cref="ClassExpression"/> should derive.
        /// </param>
        public static void SetBaseType(
            this IClassExpressionConfigurator classConfig,
            Type baseType)
        {
            classConfig.SetBaseType(baseType, configuration: null);
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseTypeExpression"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="baseTypeExpression">
        /// The base type from which the <see cref="ClassExpression"/> should derive.
        /// </param>
        public static void SetBaseType(
            this IClassExpressionConfigurator classConfig,
            ClassExpression baseTypeExpression)
        {
            classConfig.SetBaseType(baseTypeExpression, configuration: null);
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseType"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="baseType">
        /// The base Type from which the <see cref="ClassExpression"/> should derive.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetBaseType(
            this IClassExpressionConfigurator classConfig,
            Type baseType,
            Action<IClassImplementationConfigurator> configuration)
        {
            ThrowIfInvalidBaseType(baseType);

            classConfig.SetBaseType(TypeExpressionFactory.CreateClass(baseType), configuration);
        }

        private static void ThrowIfInvalidBaseType(Type baseType)
        {
            if (!baseType.IsClass() || baseType.IsSealed())
            {
                throw new InvalidOperationException(
                    $"Type '{baseType.GetFriendlyName()}' is not a valid base type.");
            }
        }

        /// <summary>
        /// Configures the <see cref="InterfaceExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">The interface Type the <see cref="InterfaceExpression"/> should implement.</typeparam>
        /// <param name="interfaceConfig">The <see cref="IInterfaceExpressionConfigurator"/> to configure.</param>
        public static void SetImplements<TInterface>(
            this IInterfaceExpressionConfigurator interfaceConfig)
        {
            interfaceConfig.SetImplements(typeof(TInterface));
        }

        /// <summary>
        /// Configures the <see cref="InterfaceExpression"/> to implement the given
        /// <paramref name="interface"/>.
        /// </summary>
        /// <param name="interface">The interface Type the <see cref="InterfaceExpression"/> should implement.</param>
        /// <param name="interfaceConfig">The <see cref="IInterfaceExpressionConfigurator"/> to configure.</param>
        public static void SetImplements(
            this IInterfaceExpressionConfigurator interfaceConfig,
            Type @interface)
        {
            interfaceConfig.SetImplements(@interface, configuration: null);
        }

        /// <summary>
        /// Configures the <see cref="InterfaceExpression"/> to implement the given
        /// <paramref name="interface"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interface">The interface Type the <see cref="InterfaceExpression"/> should implement.</param>
        /// <param name="interfaceConfig">The <see cref="IInterfaceExpressionConfigurator"/> to configure.</param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements(
            this IInterfaceExpressionConfigurator interfaceConfig,
            Type @interface,
            Action<IImplementationConfigurator> configuration)
        {
            ThrowIfNonInterfaceType(@interface);

            var interfaceExpression = new TypedInterfaceExpression(@interface);
            interfaceConfig.SetImplements(interfaceExpression, configuration);
        }

        private static void ThrowIfNonInterfaceType(Type @interface)
        {
            @interface.ThrowIfNull(nameof(@interface));

            if (!@interface.IsInterface())
            {
                throw new ArgumentException(
                    $"Type '{@interface.GetFriendlyName()}' is not an interface type.",
                    nameof(@interface));
            }
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TInterface">The interface Type <see cref="ClassExpression"/> should implement.</typeparam>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements<TInterface>(
            this IClassExpressionConfigurator classConfig,
            Action<IClassImplementationConfigurator> configuration)
        {
            classConfig.SetImplements(typeof(TInterface), configuration);
        }

        /// <summary>
        /// Configures the <see cref="StructExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TInterface">The interface Type <see cref="StructExpression"/> should implement.</typeparam>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements<TInterface>(
            this IStructExpressionConfigurator structConfig,
            Action<IStructImplementationConfigurator> configuration)
        {
            structConfig.SetImplements(typeof(TInterface), configuration);
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to implement the given
        /// <paramref name="interface"/> Type, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="interface">The interface Type the <see cref="ClassExpression"/> should implement.</param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements(
            this IClassExpressionConfigurator classConfig,
            Type @interface,
            Action<IClassImplementationConfigurator> configuration)
        {
            ThrowIfNonInterfaceType(@interface);

            var interfaceExpression = new TypedInterfaceExpression(@interface);
            classConfig.SetImplements(interfaceExpression, configuration);
        }

        /// <summary>
        /// Configures the <see cref="StructExpression"/> to implement the given
        /// <paramref name="interface"/> Type, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="interface">The interface Type the <see cref="StructExpression"/> should implement.</param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements(
            this IStructExpressionConfigurator structConfig,
            Type @interface,
            Action<IStructImplementationConfigurator> configuration)
        {
            var interfaceExpression = new TypedInterfaceExpression(@interface);
            structConfig.SetImplements(interfaceExpression, configuration);
        }

        /// <summary>
        /// Adds a ParameterExpression with the given <typeparamref name="TParameter"/> Type and
        /// <paramref name="name"/> to the <see cref="ConstructorExpression"/>.
        /// </summary>
        /// <typeparam name="TParameter">The type of the ParameterExpression to add.</typeparam>
        /// <param name="ctorConfig">The <see cref="IConstructorExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        /// <returns>The newly-added ParameterExpression.</returns>
        public static ParameterExpression AddParameter<TParameter>(
            this IConstructorExpressionConfigurator ctorConfig,
            string name)
        {
            return ctorConfig.AddParameter(name, typeof(TParameter));
        }

        /// <summary>
        /// Adds a ParameterExpression with the given <paramref name="name"/> and
        /// <paramref name="type"/> to the <see cref="ConstructorExpression"/>.
        /// </summary>
        /// <param name="ctorConfig">The <see cref="IConstructorExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        /// <param name="type">The type of the ParameterExpression to add.</param>
        /// <returns>The newly-added ParameterExpression.</returns>
        public static ParameterExpression AddParameter(
            this IConstructorExpressionConfigurator ctorConfig,
            string name,
            Type type)
        {
            name.ThrowIfInvalidName("Parameter");

            var parameterExpression = Expression.Parameter(type, name);
            ctorConfig.AddParameter(parameterExpression);
            return parameterExpression;
        }

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to the <see cref="ConstructorExpression"/>.
        /// </summary>
        /// <param name="ctorConfig">The <see cref="IConstructorExpressionConfigurator"/> to configure.</param>
        /// <param name="parameter">The ParameterExpression to add.</param>
        public static void AddParameter(
            this IConstructorExpressionConfigurator ctorConfig,
            ParameterExpression parameter)
        {
            parameter.ThrowIfNull(nameof(parameter));
            ctorConfig.AddParameters(parameter);
        }

        /// <summary>
        /// Adds a call from the <see cref="ConstructorExpression"/> to the given sibling or base
        /// Type <paramref name="targetConstructor"/>.
        /// </summary>
        /// <param name="ctorConfig">The <see cref="IConstructorExpressionConfigurator"/> to configure.</param>
        /// <param name="targetConstructor">The sibling or base Type ConstructorInfo to call.</param>
        /// <param name="arguments">
        /// Zero or more Expressions to pass to the given sibling or base Type
        /// <paramref name="targetConstructor"/>.
        /// </param>
        public static void SetConstructorCall(
            this IConstructorExpressionConfigurator ctorConfig,
            ConstructorInfo targetConstructor,
            params Expression[] arguments)
        {
            var ctorExpression = new ConstructorInfoConstructorExpression(
                TypeExpressionFactory.Create(targetConstructor.DeclaringType),
                targetConstructor);

            ctorConfig.SetConstructorCall(ctorExpression, arguments);
        }

        /// <summary>
        /// Set the summary documentation of the <see cref="SourceCode.MemberExpression"/>.
        /// </summary>
        /// <param name="memberConfig">The <see cref="IMemberExpressionConfigurator"/> to configure.</param>
        /// <param name="summary">The summary documentation of the <see cref="SourceCode.MemberExpression"/>.</param>
        public static void SetSummary(
            this IMemberExpressionConfigurator memberConfig,
            string summary)
        {
            memberConfig.SetSummary(ReadableExpression.Comment(summary));
        }

        /// <summary>
        /// Add a member to the <see cref="EnumExpression"/> with the given <paramref name="name"/>
        /// and an auto-generated value.
        /// </summary>
        /// <param name="enumConfig">The <see cref="IEnumExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the enum member.</param>
        public static void AddMember(
            this IEnumExpressionConfigurator enumConfig,
            string name)
        {
            enumConfig.AddMembers(name);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="FieldExpression"/> to the
        /// <see cref="ConcreteTypeExpression"/>, with the given <paramref name="name"/> and
        /// <typeparamref name="TField"/> type.
        /// </summary>
        /// <typeparam name="TField">The type of the <see cref="FieldExpression"/>.</typeparam>
        /// <param name="concreteTypeConfig">
        /// The <see cref="IConcreteTypeExpressionConfigurator"/> to configure.
        /// </param>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        public static FieldExpression AddField<TField>(
            this IConcreteTypeExpressionConfigurator concreteTypeConfig,
            string name)
        {
            return concreteTypeConfig.AddField<TField>(name, _ => { });
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="FieldExpression"/> to the
        /// <see cref="ConcreteTypeExpression"/>, with the given <paramref name="name"/>,
        /// <typeparamref name="TField"/> type and <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TField">The type of the <see cref="FieldExpression"/>.</typeparam>
        /// <param name="concreteTypeConfig">
        /// The <see cref="IConcreteTypeExpressionConfigurator"/> to configure.
        /// </param>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        public static FieldExpression AddField<TField>(
            this IConcreteTypeExpressionConfigurator concreteTypeConfig,
            string name,
            Action<IFieldExpressionConfigurator> configuration)
        {
            return concreteTypeConfig
                .AddField(name, BclTypeWrapper.For(typeof(TField)), configuration);
        }

        /// <summary>
        /// Set the <see cref="FieldExpression"/>'s initial value to the given
        /// <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to set.</typeparam>
        /// <param name="fieldConfig">The <see cref="IFieldExpressionConfigurator"/> to configure.</param>
        /// <param name="value">The value to which to initialise the <see cref="FieldExpression"/>.</param>
        public static void SetInitialValue<TValue>(
            this IFieldExpressionConfigurator fieldConfig,
            TValue value)
        {
            fieldConfig.SetInitialValue(Expression.Constant(value, typeof(TValue)));
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="InterfaceExpression"/>, with the given <paramref name="name"/> and
        /// <typeparamref name="TProperty"/> type.
        /// </summary>
        /// <typeparam name="TProperty">The type of the <see cref="PropertyExpression"/>.</typeparam>
        /// <param name="interfaceConfig">The <see cref="IInterfaceExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty<TProperty>(
            this IInterfaceExpressionConfigurator interfaceConfig,
            string name,
            Action<IInterfacePropertyExpressionConfigurator> configuration)
        {
            return interfaceConfig
                .AddProperty(name, BclTypeWrapper.For(typeof(TProperty)), configuration);
        }

        /// <summary>
        /// Add an implementation or override of the given <paramref name="implementedProperty"/>
        /// to the <see cref="ClassExpression"/> by creating a public auto-property with the
        /// appropriate accessor(s).
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassImplementationConfigurator"/> to configure.</param>
        /// <param name="implementedProperty">The <see cref="PropertyExpression"/> to implement.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IClassImplementationConfigurator classConfig,
            PropertyExpression implementedProperty)
        {
            return classConfig.AddProperty(implementedProperty, p =>
            {
                Implement(implementedProperty, p);
            });
        }

        /// <summary>
        /// Add an implementation or override of the given <paramref name="implementedProperty"/>
        /// to the <see cref="ClassExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassImplementationConfigurator"/> to configure.</param>
        /// <param name="implementedProperty">The <see cref="PropertyExpression"/> to implement.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IClassImplementationConfigurator classConfig,
            PropertyExpression implementedProperty,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return classConfig.AddProperty(
                implementedProperty.Name,
              ((IProperty)implementedProperty).Type,
                configuration);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="ClassExpression"/>, with the given <paramref name="name"/> and
        /// <typeparamref name="TProperty"/> type.
        /// </summary>
        /// <typeparam name="TProperty">The type of the <see cref="PropertyExpression"/>.</typeparam>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty<TProperty>(
            this IClassExpressionConfigurator classConfig,
            string name)
        {
            return classConfig.AddProperty(name, typeof(TProperty));
        }

        /// <summary>
        /// Add a <see cref="PropertyExpression"/> to the <see cref="ClassExpression"/>, with the
        /// given <paramref name="name"/>, <typeparamref name="TProperty"/> type and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the <see cref="PropertyExpression"/>.</typeparam>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty<TProperty>(
            this IClassExpressionConfigurator classConfig,
            string name,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return classConfig.AddProperty(name, typeof(TProperty), configuration);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="ClassExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The type of the <see cref="PropertyExpression"/>.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IClassExpressionConfigurator classConfig,
            string name,
            Type type)
        {
            return classConfig.AddProperty(name, type, PublicGetSet);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="ClassExpression"/>, with the given <paramref name="name"/>,
        /// <paramref name="type"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The Type of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IClassExpressionConfigurator classConfig,
            string name,
            Type type,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return classConfig.AddProperty(name, BclTypeWrapper.For(type), configuration);
        }

        /// <summary>
        /// Add an override of the given <paramref name="overriddenProperty"/> to the
        /// <see cref="ClassExpression"/> by creating a public auto-property with the appropriate
        /// accessor(s).
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="overriddenProperty">The <see cref="PropertyExpression"/> to override.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IClassExpressionConfigurator classConfig,
            PropertyExpression overriddenProperty)
        {
            return classConfig.AddProperty(overriddenProperty, p =>
            {
                Implement(overriddenProperty, p);
            });
        }

        /// <summary>
        /// Add an override of the given <paramref name="overriddenProperty"/> to the
        /// <see cref="ClassExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="overriddenProperty">The <see cref="PropertyExpression"/> to override.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IClassExpressionConfigurator classConfig,
            PropertyExpression overriddenProperty,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return classConfig.AddProperty(
                overriddenProperty.Name,
              ((IProperty)overriddenProperty).Type,
                configuration);
        }

        /// <summary>
        /// Add an implementation or override of the given <paramref name="implementedProperty"/>
        /// to the <see cref="StructExpression"/> by creating a public auto-property with the
        /// appropriate accessor(s).
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructImplementationConfigurator"/> to configure.</param>
        /// <param name="implementedProperty">The <see cref="PropertyExpression"/> to implement.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IStructImplementationConfigurator structConfig,
            PropertyExpression implementedProperty)
        {
            return structConfig.AddProperty(implementedProperty, p =>
            {
                Implement(implementedProperty, p);
            });
        }

        private static void Implement(
            PropertyExpression property,
            IConcreteTypePropertyExpressionConfigurator propertyConfig)
        {
            if (property.GetterExpression != null)
            {
                propertyConfig.SetGetter();
            }

            if (property.SetterExpression != null)
            {
                propertyConfig.SetSetter();
            }
        }

        /// <summary>
        /// Add an implementation or override of the given <paramref name="implementedProperty"/>
        /// to the <see cref="StructExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructImplementationConfigurator"/> to configure.</param>
        /// <param name="implementedProperty">The <see cref="PropertyExpression"/> to implement.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IStructImplementationConfigurator structConfig,
            PropertyExpression implementedProperty,
            Action<IConcreteTypePropertyExpressionConfigurator> configuration)
        {
            return structConfig.AddProperty(
                implementedProperty.Name,
              ((IProperty)implementedProperty).Type,
                configuration);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="StructExpression"/>, with the given <paramref name="name"/> and
        /// <typeparamref name="TProperty"/> type.
        /// </summary>
        /// <typeparam name="TProperty">The type of the <see cref="PropertyExpression"/>.</typeparam>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty<TProperty>(
            this IStructExpressionConfigurator structConfig,
            string name)
        {
            return structConfig.AddProperty(name, typeof(TProperty));
        }

        /// <summary>
        /// Add a <see cref="PropertyExpression"/> to the <see cref="StructExpression"/>, with the
        /// given <paramref name="name"/>, <typeparamref name="TProperty"/> type and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the <see cref="PropertyExpression"/>.</typeparam>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty<TProperty>(
            this IStructExpressionConfigurator structConfig,
            string name,
            Action<IConcreteTypePropertyExpressionConfigurator> configuration)
        {
            return structConfig
                .AddProperty(name, BclTypeWrapper.For(typeof(TProperty)), configuration);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="StructExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The type of the <see cref="PropertyExpression"/>.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IStructExpressionConfigurator structConfig,
            string name,
            Type type)
        {
            return structConfig.AddProperty(name, type, PublicGetSet);
        }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="PropertyExpression"/> to the
        /// <see cref="StructExpression"/>, with the given <paramref name="name"/>,
        /// <paramref name="type"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The type of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        public static PropertyExpression AddProperty(
            this IStructExpressionConfigurator structConfig,
            string name,
            Type type,
            Action<IConcreteTypePropertyExpressionConfigurator> configuration)
        {
            return structConfig.AddProperty(name, BclTypeWrapper.For(type), configuration);
        }

        internal static void PublicGetSet(
            this IConcreteTypePropertyExpressionConfigurator propertyConfig)
        {
            propertyConfig.SetGetter();
            propertyConfig.SetSetter();
        }

        /// <summary>
        /// Add an auto-property getter with the optional given <paramref name="visibility"/> to the
        /// <see cref="PropertyExpression"/>.
        /// </summary>
        /// <param name="propertyConfig">
        /// The <see cref="IConcreteTypePropertyExpressionConfigurator"/> to configure.
        /// </param>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> of the auto-property getter to add. Defaults to
        /// MemberVisibility.Public.
        /// </param>
        public static void SetGetter(
            this IConcreteTypePropertyExpressionConfigurator propertyConfig,
            MemberVisibility? visibility = null)
        {
            propertyConfig.SetGetter(p => p.SetVisibility(
                visibility ?? ((PropertyExpression)propertyConfig).Visibility ?? Public));
        }

        /// <summary>
        /// Add an auto-property setter with the optional given <paramref name="visibility"/> to the
        /// <see cref="PropertyExpression"/>.
        /// </summary>
        /// <param name="propertyConfig">
        /// The <see cref="IConcreteTypePropertyExpressionConfigurator"/> to configure.
        /// </param>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> of the auto-property setter to add. Defaults to
        /// MemberVisibility.Public.
        /// </param>
        public static void SetSetter(
            this IConcreteTypePropertyExpressionConfigurator propertyConfig,
            MemberVisibility? visibility = null)
        {
            propertyConfig.SetSetter(p => p.SetVisibility(
                visibility ?? ((PropertyExpression)propertyConfig).Visibility ?? Public));
        }

        /// <summary>
        /// Add auto-property get and set accessors with the given <paramref name="getterVisibility"/>
        /// and <paramref name="setterVisibility"/> to the <see cref="PropertyExpression"/>.
        /// </summary>
        /// <param name="propertyConfig">
        /// The <see cref="IConcreteTypePropertyExpressionConfigurator"/> to configure.
        /// </param>
        /// <param name="getterVisibility">
        /// The <see cref="MemberVisibility"/> of the auto-property getter to add. Defaults to
        /// MemberVisibility.Public.
        /// </param>
        /// <param name="setterVisibility">
        /// The <see cref="MemberVisibility"/> of the auto-property setter to add. Defaults to
        /// MemberVisibility.Public.
        /// </param>
        /// <param name="isStatic">
        /// A value indicating whether the auto-property shold have static scope. Defaults to false.
        /// </param>
        public static void SetAutoProperty(
            this IConcreteTypePropertyExpressionConfigurator propertyConfig,
            MemberVisibility? getterVisibility = null,
            MemberVisibility? setterVisibility = null,
            bool isStatic = false)
        {
            if (isStatic)
            {
                propertyConfig.SetStatic();
            }

            propertyConfig.SetGetter(getterVisibility);
            propertyConfig.SetSetter(setterVisibility);
        }

        /// <summary>
        /// Add a public, instance-scoped <see cref="MethodExpression"/> to the
        /// <see cref="ClassExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="body"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassImplementationConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IClassImplementationConfigurator classConfig,
            string name,
            Expression body)
        {
            return classConfig.AddMethod(name, m => m.SetBody(body));
        }

        /// <summary>
        /// Add a public, instance-scoped <see cref="MethodExpression"/> to the
        /// <see cref="StructExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="body"/>.
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructImplementationConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IStructImplementationConfigurator structConfig,
            string name,
            Expression body)
        {
            return structConfig.AddMethod(name, cfg => cfg.SetBody(body));
        }

        /// <summary>
        /// Add an override of the given <paramref name="overriddenMethod"/> to the
        /// <see cref="ClassExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassImplementationConfigurator"/> to configure.</param>
        /// <param name="overriddenMethod">The <see cref="MethodExpression"/> to override.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IClassImplementationConfigurator classConfig,
            MethodExpression overriddenMethod,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return classConfig.AddMethod(overriddenMethod.Name, m =>
            {
                if (overriddenMethod.ParametersAccessor != null)
                {
                    m.AddParameters(overriddenMethod.ParametersAccessor);
                }

                configuration.Invoke(m);
            });
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
            return interfaceConfig.AddMethod(name, returnType, _ => { });
        }

        /// <summary>
        /// Add a parameterless <see cref="MethodExpression"/> to the
        /// <see cref="InterfaceExpression"/>, with the given <paramref name="name"/>,
        /// <paramref name="returnType"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="interfaceConfig">The <see cref="IInterfaceExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="returnType">The return type of the <see cref="MethodExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IInterfaceExpressionConfigurator interfaceConfig,
            string name,
            Type returnType,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return interfaceConfig
                .AddMethod(name, BclTypeWrapper.For(returnType), configuration);
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
        /// Add an override of the given <paramref name="overriddenMethod"/> to the
        /// <see cref="ClassExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="overriddenMethod">The <see cref="MethodExpression"/> to override.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this IClassExpressionConfigurator classConfig,
            MethodExpression overriddenMethod,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return classConfig.AddMethod(overriddenMethod.Name, m =>
            {
                if (overriddenMethod.ParametersAccessor != null)
                {
                    m.AddParameters(overriddenMethod.ParametersAccessor);
                }

                configuration.Invoke(m);
            });
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
            return methodConfig.AddGenericParameter(name, _ => { });
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
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <paramref name="types"/>.
        /// </summary>
        /// <param name="parameterConfig">The <see cref="IGenericParameterExpressionConfigurator"/> to configure.</param>
        /// <param name="types">The Types to which to constrain the <see cref="GenericParameterExpression"/>.</param>
        public static void AddTypeConstraints(
            this IGenericParameterExpressionConfigurator parameterConfig,
            params Type[] types)
        {
            parameterConfig.AddTypeConstraints(types.ProjectToArray(TypeExpressionFactory.Create));
        }

        /// <summary>
        /// Add a ParameterExpression to the <see cref="MethodExpression"/> with the given
        /// <paramref name="name"/> and <typeparamref name="TParameter"/>.
        /// </summary>
        /// <typeparam name="TParameter">The type of the ParameterExpression to add.</typeparam>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        public static void AddParameter<TParameter>(
            this IMethodExpressionConfigurator methodConfig,
            string name)
        {
            methodConfig.AddParameter(typeof(TParameter), name);
        }

        /// <summary>
        /// Add a ParameterExpression to the <see cref="MethodExpression"/> with the given
        /// <paramref name="name"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="type">The type of the ParameterExpression to add.</param>
        /// <param name="name">The name of the ParameterExpression to add.</param>
        public static void AddParameter(
            this IMethodExpressionConfigurator methodConfig,
            Type type,
            string name)
        {
            methodConfig.AddParameter(Expression.Parameter(type, name));
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
        /// Mark the class <see cref="MethodExpression"/> as abstract, with the return type
        /// <typeparamref name="TReturn"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type to apply to the <see cref="MethodExpression"/>.</typeparam>
        /// <param name="methodConfig">The <see cref="IClassMethodExpressionConfigurator"/> to configure.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the class
        /// <see cref="MethodExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MethodExpression"/> has already been marked as static.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MethodExpression"/> has already been marked as virtual.
        /// </exception>
        public static void SetAbstract<TReturn>(
            this IClassMethodExpressionConfigurator methodConfig)
        {
            methodConfig.SetAbstract(typeof(TReturn));
        }

        /// <summary>
        /// Mark the class <see cref="MethodExpression"/> as abstract, with the given
        /// <paramref name="returnType"/>.
        /// </summary>
        /// <param name="methodConfig">The <see cref="IClassMethodExpressionConfigurator"/> to configure.</param>
        /// <param name="returnType">The return type to apply to the <see cref="MethodExpression"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="ClassExpression"/> which declares the class
        /// <see cref="MethodExpression"/> has not been marked as abstract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MethodExpression"/> has already been marked as static.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the class <see cref="MethodExpression"/> has already been marked as virtual.
        /// </exception>
        public static void SetAbstract(
            this IClassMethodExpressionConfigurator methodConfig,
            Type returnType)
        {
            methodConfig.SetAbstract(BclTypeWrapper.For(returnType));
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
            var returnType = body.NodeType == ExpressionType.Lambda
                ? ((LambdaExpression)body).ReturnType
                : body.Type;

            methodConfig.SetBody(body, returnType);
        }
    }
}