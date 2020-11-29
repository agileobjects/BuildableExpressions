namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using Api;
    using BuildableExpressions.Extensions;
    using Generics;
    using ReadableExpressions;
    using ReadableExpressions.Translations.Reflection;

    public partial class TypeExpression : ITypeableTypeExpressionConfigurator
    {
        internal virtual void SetImplements(
            InterfaceExpression interfaceExpression,
            Action<ImplementationConfigurator> configuration)
        {
            interfaceExpression.ThrowIfNull(nameof(interfaceExpression));

            if (configuration != null)
            {
                var configurator = new ImplementationConfigurator(
                    this,
                    interfaceExpression,
                    configuration);

                interfaceExpression = (InterfaceExpression)configurator.ImplementedTypeExpression;
            }

            SetImplements(interfaceExpression);
        }

        internal void SetImplements(InterfaceExpression interfaceExpression)
        {
            _interfaceExpressions ??= new List<InterfaceExpression>();
            _interfaceExpressions.Add(interfaceExpression);
            _readOnlyInterfaceExpressions = null;
            _readOnlyInterfaceTypes = null;
        }

        void ITypeExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void ITypeExpressionConfigurator.SetVisibility(TypeVisibility visibility)
            => Visibility = visibility;

        OpenGenericParameterExpression IGenericParameterConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            return AddGenericParameter(new ConfiguredOpenGenericParameterExpression(
                SourceCode,
                name,
                configuration));
        }

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="OpenGenericParameterExpression"/> to add.</param>
        /// <returns>The given <paramref name="parameter"/>.</returns>
        protected OpenGenericParameterExpression AddGenericParameter(
            OpenGenericParameterExpression parameter)
        {
            _genericParameters ??= new List<OpenGenericParameterExpression>();
            _genericParameters.Add(parameter);
            _readOnlyGenericParameters = null;

            _genericArguments ??= new List<IType>();
            _genericArguments.Add(parameter);
            _readOnlyGenericArguments = null;

            ResetTypeIfRequired();
            return parameter;
        }

        internal PropertyExpression AddProperty(
            string name,
            IType type,
            Action<StandardPropertyExpression> configuration)
        {
            return AddProperty(new StandardPropertyExpression(this, name, type, configuration));
        }

        /// <summary>
        /// Adds the given <paramref name="property"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="property">The <see cref="PropertyExpression"/> to add.</param>
        /// <returns>The given <paramref name="property"/>.</returns>
        protected PropertyExpression AddProperty(PropertyExpression property)
        {
            _propertyExpressions ??= new List<PropertyExpression>();
            _propertyExpressions.Add(property);
            _readOnlyPropertyExpressions = null;
            AddMember(property);
            return property;
        }

        internal MethodExpression AddMethod(
            string name,
            Action<StandardMethodExpression> configuration)
        {
            return AddMethod(new StandardMethodExpression(this, name, configuration));
        }

        internal virtual StandardMethodExpression AddMethod(
            StandardMethodExpression method)
        {
            AddMethod((MethodExpression)method);

            if (!method.HasBlockMethods)
            {
                return method;
            }

            foreach (var blockMethod in method.BlockMethods)
            {
                blockMethod.Finalise();
                AddMethod(blockMethod);
            }

            return method;
        }

        internal MethodExpression AddMethod(MethodExpression method)
        {
            _methodExpressions ??= new List<MethodExpression>();
            _methodExpressions.Add(method);
            _readOnlyMethodExpressions = null;
            AddMember(method);
            return method;
        }

        private void AddMember(MemberExpression member)
        {
            _memberExpressions.Add(member);
            _readOnlyMemberExpressions = null;
            _members = null;
            ResetTypeIfRequired();
        }

        private void ResetTypeIfRequired()
        {
            if (_type != null)
            {
                _rebuildType = true;
            }
        }
    }
}
