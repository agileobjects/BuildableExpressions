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

        void ITypeExpressionConfigurator.SetPartial() => IsPartial = true;

        void ITypeExpressionConfigurator.SetSummary(string summary)
            => Summary = ReadableExpression.Comment(summary);

        void ITypeExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void ITypeExpressionConfigurator.SetVisibility(TypeVisibility visibility)
            => Visibility = visibility;

        #region ITypeableTypeExpressionConfigurator Members

        void ITypeableTypeExpressionConfigurator.AddGenericParameter(
            GenericParameterExpression parameter)
        {
            AddGenericParameter(parameter);
        }

        #endregion

        GenericParameterExpression IGenericParameterConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            return AddGenericParameter(new ConfiguredGenericParameterExpression(
               (ConfiguredSourceCodeExpression)SourceCode,
                name,
                configuration));
        }

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        /// <returns>The given <paramref name="parameter"/>.</returns>
        protected GenericParameterExpression AddGenericParameter(
            GenericParameterExpression parameter)
        {
            _genericParameters ??= new List<GenericParameterExpression>();
            _genericParameters.Add(parameter);
            _readOnlyGenericParameters = null;

            _genericArguments ??= new List<TypeExpression>();
            _genericArguments.Add(parameter);
            _readOnlyGenericArguments = null;

            ResetTypeIfRequired();
            return parameter;
        }

        /// <summary>
        /// Adds a parameterless, empty <see cref="ConstructorExpression"/> to this
        /// <see cref="TypeExpression"/>.
        /// </summary>
        protected void AddDefaultConstructor()
            => _defaultCtorExpression = AddConstructor(ctor => ctor.SetBody(Empty()));

        private void RemoveDefaultConstructorIfPresent()
        {
            if (_defaultCtorExpression == null)
            {
                return;
            }

            _ctorExpressions.Remove(_defaultCtorExpression);
            _readOnlyCtorExpressions = null;

            _memberExpressions.Remove(_defaultCtorExpression);
            _readOnlyMemberExpressions = null;
            _members = null;

            _defaultCtorExpression = null;
            ResetTypeIfRequired();
        }

        internal ConstructorExpression AddConstructor(Action<ConfiguredConstructorExpression> configuration)
            => AddConstructor(new ConfiguredConstructorExpression(this, configuration));

        /// <summary>
        /// Adds the given <paramref name="ctor"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="ctor">The <see cref="ConstructorExpression"/> to add.</param>
        /// <returns>The given <paramref name="ctor"/>.</returns>
        protected ConstructorExpression AddConstructor(ConstructorExpression ctor)
        {
            _ctorExpressions ??= new List<ConstructorExpression>();
            _ctorExpressions.Add(ctor);
            _readOnlyCtorExpressions = null;

            if (ctor.ParametersAccessor == null)
            {
                RemoveDefaultConstructorIfPresent();
            }

            return AddMember(ctor);
        }

        /// <summary>
        /// Add a <see cref="FieldExpression"/> to this <see cref="TypeExpression"/>, with the
        /// given <paramref name="name"/>, <paramref name="type"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <param name="type">The <see cref="IType"/> of the <see cref="FieldExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        protected FieldExpression AddField(
            string name,
            IType type,
            Action<FieldExpression> configuration)
        {
            var field = new FieldExpression(this, name, type, configuration);

            _fieldExpressions ??= new List<FieldExpression>();
            _fieldExpressions.Add(field);
            _readOnlyFieldExpressions = null;

            return AddMember(field);
        }

        internal PropertyExpression AddProperty(
            string name,
            IType type,
            Action<ConfiguredPropertyExpression> configuration)
        {
            return AddProperty(new ConfiguredPropertyExpression(this, name, type, configuration));
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

            return AddMember(property);
        }

        internal MethodExpression AddMethod(
            string name,
            Action<ConfiguredMethodExpression> configuration)
        {
            return AddMethod(new ConfiguredMethodExpression(this, name, configuration));
        }

        internal void AddMethod(BlockMethodExpression blockMethod)
        {
            _blockMethodExpressions ??= new List<BlockMethodExpression>();
            _blockMethodExpressions.Add(blockMethod);
        }

        internal virtual MethodExpression AddMethod(MethodExpression method)
        {
            _methodExpressions ??= new List<MethodExpression>();
            _methodExpressions.Add(method);
            _readOnlyMethodExpressions = null;

            return AddMember(method);
        }

        private TMemberExpression AddMember<TMemberExpression>(TMemberExpression member)
            where TMemberExpression : MemberExpression
        {
            member.MemberIndex = _memberExpressions.Count;

            _memberExpressions.Add(member);
            _readOnlyMemberExpressions = null;
            _members = null;

            ResetTypeIfRequired();
            return member;
        }

        private void ResetTypeIfRequired()
        {
            if (_type != null)
            {
                ResetType();
            }
        }

        /// <summary>
        /// Resets the generated CLR Type and any associated members, as changes have been made to
        /// this <see cref="TypeExpression"/> which mean a new Type must be generated.
        /// </summary>
        protected virtual void ResetType()
        {
            _rebuildType = true;

            foreach (var memberExpression in MemberExpressionsAccessor)
            {
                memberExpression.ResetMemberInfo();
            }
        }
    }
}
