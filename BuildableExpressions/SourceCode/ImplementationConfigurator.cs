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
        IStructImplementationConfigurator
    {
        private readonly TypeExpression _implementingTypeExpression;

        internal ImplementationConfigurator(
            TypeExpression implementingTypeExpression,
            TypeExpression implementedTypeExpression,
            Action<ImplementationConfigurator> configuration)
        {
            _implementingTypeExpression = implementingTypeExpression;
            ImplementedTypeExpression = implementedTypeExpression;

            configuration.Invoke(this);
        }

        internal TypeExpression ImplementedTypeExpression { get; private set; }

        public void SetGenericArgument(
            string genericParameterName,
            TypeExpression closedTypeExpression)
        {
            var genericParameter = ImplementedTypeExpression
                .GenericParameters
                .FirstOrDefault(p => p.Name == genericParameterName);

            if (genericParameter != null)
            {
                SetGenericArgument(genericParameter, closedTypeExpression);
                return;
            }

            throw new InvalidOperationException(
                $"Type '{ImplementedTypeExpression.GetFriendlyName()}' has no " +
                $"generic parameter named '{genericParameterName}'.");
        }

        public void SetGenericArgument(
            GenericParameterExpression genericParameter,
            TypeExpression closedType)
        {
            ImplementedTypeExpression = ImplementedTypeExpression
                .Close(genericParameter, closedType);
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