namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure how a <see cref="TypeExpression"/> implements a base type or
    /// interface.
    /// </summary>
    public interface IImplementationConfigurator
    {
        /// <summary>
        /// Closes the given <paramref name="parameter"/> to the given <paramref name="type"/> for
        /// the implementation.
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to
        /// close to the given <paramref name="type"/>.
        /// </param>
        /// <param name="type">The Type to which to close the given <paramref name="parameter"/>.</param>
        void SetGenericArgument(
            GenericParameterExpression parameter,
            Type type);
    }
}