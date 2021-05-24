namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;
    using Api;
    using Generics;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class ImplementationConfigurator :
        IClassImplementationConfigurator,
        IAttributeImplementationConfigurator,
        IStructImplementationConfigurator
    {
        private readonly TypeExpression _implementingTypeExpression;
        private IClosableTypeExpression _closableImplementedTypeExpression;

        internal ImplementationConfigurator(
            TypeExpression implementingTypeExpression,
            IClosableTypeExpression implementedTypeExpression,
            Action<ImplementationConfigurator> configuration)
            : this(implementingTypeExpression, implementedTypeExpression)
        {
            _closableImplementedTypeExpression = implementedTypeExpression;
            configuration.Invoke(this);
        }

        internal ImplementationConfigurator(
            TypeExpression implementingTypeExpression,
            IType implementedTypeExpression,
            Action<ImplementationConfigurator> configuration)
            : this(implementingTypeExpression, implementedTypeExpression)
        {
            configuration.Invoke(this);
        }

        private ImplementationConfigurator(
            TypeExpression implementingTypeExpression,
            IType implementedTypeExpression)
        {
            _implementingTypeExpression = implementingTypeExpression;
            ImplementedTypeExpression = implementedTypeExpression;
        }

        internal IType ImplementedTypeExpression { get; private set; }

        public void SetGenericArgument(
            string genericParameterName,
            TypeExpression closedTypeExpression)
        {
            var genericParameter = _closableImplementedTypeExpression
                .GenericParameters
                .FirstOrDefault(p => p.Name == genericParameterName);

            if (genericParameter != null)
            {
                SetGenericArgument(genericParameter, closedTypeExpression);
                return;
            }

            throw new InvalidOperationException(
                $"Type '{_closableImplementedTypeExpression.GetFriendlyName()}' has no " +
                $"generic parameter named '{genericParameterName}'.");
        }

        public void SetGenericArgument(
            GenericParameterExpression genericParameter,
            TypeExpression closedType)
        {
            ImplementedTypeExpression = _closableImplementedTypeExpression =
                _closableImplementedTypeExpression.Close(genericParameter, closedType);
        }

        PropertyExpression IClassMemberConfigurator.AddProperty(
            string name,
            IType type,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return _implementingTypeExpression.AddProperty(name, type, configuration);
        }

        MethodExpression IClassMemberConfigurator.AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return _implementingTypeExpression.AddMethod(name, configuration);
        }

        PropertyExpression IStructMemberConfigurator.AddProperty(
            string name,
            IType type,
            Action<IConcreteTypePropertyExpressionConfigurator> configuration)
        {
            return _implementingTypeExpression.AddProperty(name, type, configuration);
        }

        MethodExpression IStructMemberConfigurator.AddMethod(
            string name,
            Action<IConcreteTypeMethodExpressionConfigurator> configuration)
        {
            return _implementingTypeExpression.AddMethod(name, configuration);
        }
    }
}