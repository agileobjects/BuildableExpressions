namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using Generics;

    /// <summary>
    /// Provides options to configure how a <see cref="TypeExpression"/> implements a base type or
    /// interface.
    /// </summary>
    public interface IImplementationConfigurator
    {
        /// <summary>
        /// Closes the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/> to the given <paramref name="closedType"/> for
        /// the <see cref="TypeExpression"/>
        /// </summary>
        /// <param name="genericParameterName">
        /// The name of the <see cref="GenericParameterExpression"/> describing the open generic
        /// parameter to close to the given <paramref name="closedType"/>.
        /// </param>
        /// <param name="closedType">
        /// The Type to which to close the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/>.
        /// </param>
        public void SetGenericArgument(string genericParameterName, Type closedType);

        /// <summary>
        /// Closes the given <paramref name="parameter"/> to the given <paramref name="closedType"/>
        /// for the implementation.
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to
        /// close to the given <paramref name="closedType"/>.
        /// </param>
        /// <param name="closedType">The Type to which to close the given <paramref name="parameter"/>.</param>
        public void SetGenericArgument(GenericParameterExpression parameter, Type closedType);
    }
}