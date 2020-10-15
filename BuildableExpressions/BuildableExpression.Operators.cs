namespace AgileObjects.BuildableExpressions
{
    using SourceCode;

    /// <summary>
    /// Provides buildable Expression factory methods.
    /// </summary>
    public static partial class BuildableExpression
    {
        /// <summary>
        /// Create a <see cref="TypeOfOperatorExpression"/> that represents use of the typeof
        /// operator on the given <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> to which the typeof operator is being
        /// applied.
        /// </param>
        /// <returns>
        /// A <see cref="TypeOfOperatorExpression"/> representing use of the typeof operator on
        /// the given <paramref name="parameter"/>
        /// </returns>
        public static TypeOfOperatorExpression TypeOf(GenericParameterExpression parameter)
        {
            return parameter.IsClosed
                ? new TypeOfOperatorExpression(parameter.Type)
                : new TypeOfOperatorExpression(parameter.Name);
        }

        /// <summary>
        /// Create a <see cref="NameOfOperatorExpression"/> that represents use of the nameof
        /// operator on the given <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="GenericParameterExpression"/> to which the nameof operator is being
        /// applied.
        /// </param>
        /// <returns>
        /// A <see cref="NameOfOperatorExpression"/> representing use of the nameof operator on
        /// the given <paramref name="parameter"/>
        /// </returns>
        public static NameOfOperatorExpression NameOf(GenericParameterExpression parameter)
            => new NameOfOperatorExpression(parameter);
    }
}
