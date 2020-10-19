namespace AgileObjects.BuildableExpressions
{
    using ReadableExpressions;
    using SourceCode;
    using SourceCode.Api;

    /// <summary>
    /// Provides extension methods for easier API use.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Set the summary documentation of the <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="summary">The summary documentation of the <see cref="TypeExpression"/>.</param>
        public static void SetSummary(
            this ITypeExpressionConfigurator typeConfig,
            string summary)
        {
            typeConfig.SetSummary(ReadableExpression.Comment(summary));
        }

        /// <summary>
        /// Configures the <see cref="TypeExpression"/> to implement the given
        /// <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The type of interface the <see cref="TypeExpression"/> being built should implement.
        /// </typeparam>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        public static void SetImplements<TInterface>(
            this ITypeExpressionConfigurator typeConfig)
            where TInterface : class
        {
            typeConfig.SetImplements(typeof(TInterface));
        }
    }
}