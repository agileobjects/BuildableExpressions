namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq;
    using NetStandardPolyfills;

    /// <summary>
    /// Provides options to configure how a <see cref="TypeExpression"/> implements a base type or
    /// interface.
    /// </summary>
    public class ImplementationConfigurator
    {
        private readonly TypeExpression _implementedTypeExpression;
        private readonly Type _implementedType;
        private readonly Type[] _genericTypeArguments;

        internal ImplementationConfigurator(
            TypeExpression typeExpression,
            Type implementedType)
        {
            _implementedType = implementedType;
            _genericTypeArguments = implementedType.GetGenericTypeArguments();

            _implementedTypeExpression = typeExpression
                .SourceCode
                .TypeExpressions
                .FirstOrDefault(t => t.TypeAccessor == _implementedType);
        }

        internal Type GetImplementedType()
            => _implementedType.MakeGenericType(_genericTypeArguments);

        internal ClosedGenericTypeArgumentExpression GenericArgumentExpression
        {
            get;
            private set;
        }

        /// <summary>
        /// Closes the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/> to the given <paramref name="type"/> for the
        /// <see cref="TypeExpression"/>
        /// </summary>
        /// <param name="genericParameterName">
        /// The name of the <see cref="GenericParameterExpression"/> describing the open generic
        /// parameter to close to the given <paramref name="type"/>.
        /// </param>
        /// <param name="type">
        /// The Type to which to close the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/>.
        /// </param>
        public void SetGenericArgument(string genericParameterName, Type type)
        {
            var parameter = _implementedTypeExpression
                .GenericParameters
                .FirstOrDefault(p => p.Name == genericParameterName);

            SetGenericArgument(parameter, type);
        }

        /// <summary>
        /// Closes the given <paramref name="parameter"/> to the given <paramref name="type"/> for
        /// the implementation.
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to
        /// close to the given <paramref name="type"/>.
        /// </param>
        /// <param name="type">The Type to which to close the given <paramref name="parameter"/>.</param>
        public void SetGenericArgument(
            GenericParameterExpression parameter,
            Type type)
        {
            for (var i = 0; i < _genericTypeArguments.Length; ++i)
            {
                if (_genericTypeArguments[i].Name != parameter.Name)
                {
                    continue;
                }

                _genericTypeArguments[i] = type;
                GenericArgumentExpression = ((OpenGenericArgumentExpression)parameter).Close(type);
                return;
            }
        }
    }
}