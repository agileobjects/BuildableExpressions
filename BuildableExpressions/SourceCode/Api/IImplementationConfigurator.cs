namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using Generics;

    /// <summary>
    /// Provides options to configure how a <see cref="TypeExpression"/> implements a base type or
    /// interface.
    /// </summary>
    public interface IImplementationConfigurator
    {
        /// <summary>
        /// Closes the <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/> to the given
        /// <paramref name="closedTypeExpression"/> for the implementation.
        /// </summary>
        /// <param name="genericParameterName">
        /// The name of the <see cref="GenericParameterExpression"/> describing the open generic
        /// parameter to close to the given <paramref name="closedTypeExpression"/>.
        /// </param>
        /// <param name="closedTypeExpression">
        /// The <see cref="TypeExpression"/> to which to close the
        /// <see cref="GenericParameterExpression"/> with the given
        /// <paramref name="genericParameterName"/>.
        /// </param>
        public void SetGenericArgument(
            string genericParameterName,
            TypeExpression closedTypeExpression);

        /// <summary>
        /// Closes the given <paramref name="genericParameter"/> to the given
        /// <paramref name="closedTypeExpression"/> for the implementation.
        /// </summary>
        /// <param name="genericParameter">
        /// The <see cref="GenericParameterExpression"/> describing the open generic parameter to
        /// close to the given <paramref name="closedTypeExpression"/>.
        /// </param>
        /// <param name="closedTypeExpression">
        /// The <see cref="TypeExpression"/> to which to close the given
        /// <paramref name="genericParameter"/>.
        /// </param>
        public void SetGenericArgument(
            GenericParameterExpression genericParameter,
            TypeExpression closedTypeExpression);
    }
}