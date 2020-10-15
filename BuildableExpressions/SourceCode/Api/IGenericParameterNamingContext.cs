namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides information with which to name a <see cref="GenericParameterExpression"/>.
    /// </summary>
    public interface IGenericParameterNamingContext
    {
        /// <summary>
        /// Gets the index of the parameter in the set of generated class method parameters to which
        /// this <see cref="IGenericParameterNamingContext"/> relates.
        /// </summary>
        int Index { get; }
    }
}