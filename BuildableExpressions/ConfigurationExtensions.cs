namespace AgileObjects.BuildableExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using SourceCode;
    using SourceCode.Api;
    using SourceCode.Generics;

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
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <typeparamref name="TBase"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <typeparam name="TBase">
        /// The base type from which the <see cref="ClassExpression"/> being built should derive.
        /// </typeparam>
        public static void SetBaseType<TBase>(
            this IClassExpressionConfigurator classConfig)
            where TBase : class
        {
            classConfig.SetBaseType(typeof(TBase));
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
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseType"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="baseType">
        /// The base type from which the <see cref="ClassExpression"/> being built should derive.
        /// </param>
        public static void SetBaseType(
            this IClassExpressionConfigurator classConfig,
            Type baseType)
        {
            classConfig.SetBaseType(baseType, configuration: null);
        }

        /// <summary>
        /// Configures the <see cref="ClassExpression"/> to derive from the given
        /// <paramref name="baseTypeExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="baseTypeExpression">
        /// The base type from which the <see cref="ClassExpression"/> being built should derive.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetBaseType(
            this IClassExpressionConfigurator classConfig,
            ClassExpression baseTypeExpression,
            Action<IClassImplementationConfigurator> configuration)
        {
            classConfig.SetBaseType(baseTypeExpression.Type, configuration);
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
        /// Configures the <see cref="ClassExpression"/> to implement the Type of the given
        /// <paramref name="interfaceExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="classConfig">The <see cref="IClassExpressionConfigurator"/> to configure.</param>
        /// <param name="interfaceExpression">
        /// The <see cref="InterfaceExpression"/> the <see cref="ClassExpression"/> should implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements(
            this IClassExpressionConfigurator classConfig,
            InterfaceExpression interfaceExpression,
            Action<IClassImplementationConfigurator> configuration)
        {
            classConfig.SetImplements(interfaceExpression.Type, configuration);
        }

        /// <summary>
        /// Configures the <see cref="StructExpression"/> to implement the Type of the given
        /// <paramref name="interfaceExpression"/>, using the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="structConfig">The <see cref="IStructExpressionConfigurator"/> to configure.</param>
        /// <param name="interfaceExpression">
        /// The <see cref="InterfaceExpression"/> the <see cref="StructExpression"/> should implement.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        public static void SetImplements(
            this IStructExpressionConfigurator structConfig,
            InterfaceExpression interfaceExpression,
            Action<IStructImplementationConfigurator> configuration)
        {
            structConfig.SetImplements(interfaceExpression.Type, configuration);
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
            return interfaceConfig.AddProperty(name, typeof(TProperty), configuration);
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
                implementedProperty.Type,
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
                implementedProperty.Type,
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
            return structConfig.AddProperty(name, typeof(TProperty), configuration);
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

        private static void PublicGetSet(
            IConcreteTypePropertyExpressionConfigurator propertyConfig)
        {
            propertyConfig.SetGetter();
            propertyConfig.SetSetter();
        }

        /// <summary>
        /// Add an auto-property getter with the given <paramref name="visibility"/> to the
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
            MemberVisibility visibility = MemberVisibility.Public)
        {
            propertyConfig.SetGetter(p => p.SetVisibility(visibility));
        }

        /// <summary>
        /// Add an auto-property setter with the given <paramref name="visibility"/> to the
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
            MemberVisibility visibility = MemberVisibility.Public)
        {
            propertyConfig.SetSetter(p => p.SetVisibility(visibility));
        }

        /// <summary>
        /// Add auto-property accessors with the given <paramref name="getterVisibility"/> and
        /// <paramref name="setterVisibility"/> to the <see cref="PropertyExpression"/>.
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
            MemberVisibility getterVisibility = MemberVisibility.Public,
            MemberVisibility setterVisibility = MemberVisibility.Public,
            bool isStatic = false)
        {
            if (isStatic)
            {
                propertyConfig.SetStatic();
            }

            propertyConfig.SetGetter(p => p.SetVisibility(getterVisibility));
            propertyConfig.SetSetter(p => p.SetVisibility(setterVisibility));
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