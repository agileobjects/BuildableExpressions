namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;
    using Api;
    using BuildableExpressions.Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;

    internal class ImplementationConfigurator :
        IClassImplementationConfigurator,
        IStructImplementationConfigurator
    {
        private readonly TypeExpression _typeExpression;
        private readonly TypeExpression _implementedTypeExpression;
        private readonly Type _implementedType;
        private readonly Type[] _genericTypeArguments;

        internal ImplementationConfigurator(
            TypeExpression typeExpression,
            Type implementedType)
        {
            _typeExpression = typeExpression;
            _implementedType = implementedType;
            _genericTypeArguments = implementedType.GetGenericTypeArguments();

            _implementedTypeExpression = typeExpression
                .SourceCode
                .TypeExpressions
                .FirstOrDefault(t => t.TypeAccessor == _implementedType);
        }

        internal Type GetImplementedType()
        {
            return _genericTypeArguments.Length != 0 && _implementedType.IsGenericTypeDefinition()
                ? _implementedType.MakeGenericType(_genericTypeArguments)
                : _implementedType;
        }

        public void SetGenericArgument(string genericParameterName, Type closedType)
        {
            if (TryGetImplementedTypeParameterExpression(
                    genericParameterName,
                    out var parameterExpression))
            {
                SetGenericArgument(parameterExpression, closedType);
                return;
            }

            var parameterType = _genericTypeArguments
                .FirstOrDefault(arg => arg.Name == genericParameterName);

            if (parameterType != null)
            {
                SetGenericArgument(new TypedOpenGenericArgumentExpression(parameterType), closedType);
                return;
            }

            throw new InvalidOperationException(
                $"Type '{_implementedType.GetFriendlyName()}' has no " +
                $"generic parameter named '{genericParameterName}'.");
        }

        private bool TryGetImplementedTypeParameterExpression(
            string parameterName,
            out GenericParameterExpression parameterExpression)
        {
            if (_implementedTypeExpression == null)
            {
                parameterExpression = null;
                return false;
            }

            parameterExpression = _implementedTypeExpression
                .GenericParameters
                .FirstOrDefault(p => p.Name == parameterName);

            return parameterExpression != null;
        }

        public void SetGenericArgument(
            GenericParameterExpression parameter,
            Type closedType)
        {
            SetGenericArgument((OpenGenericArgumentExpression)parameter, closedType);
        }

        private void SetGenericArgument(
            OpenGenericArgumentExpression parameter,
            Type closedType)
        {
            for (var i = 0; i < _genericTypeArguments.Length; ++i)
            {
                if (_genericTypeArguments[i].Name != parameter.Name)
                {
                    continue;
                }

                _genericTypeArguments[i] = closedType;
                _typeExpression.AddGenericArgument(parameter.Close(closedType));
                return;
            }
        }

        PropertyOrFieldExpression IClassMemberConfigurator.AddProperty(
            string name,
            Type type,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return ((ClassExpression)_typeExpression).AddProperty(name, type, configuration);
        }

        MethodExpression IClassMemberConfigurator.AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return ((ClassExpression)_typeExpression).AddMethod(name, configuration);
        }

        MethodExpression IStructMemberConfigurator.AddMethod(
            string name,
            Action<IConcreteTypeMethodExpressionConfigurator> configuration)
        {
            return ((StructExpression)_typeExpression).AddMethod(name, configuration);
        }
    }
}