namespace AgileObjects.BuildableExpressions
{
    using System.Linq.Expressions;
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

        /// <summary>
        /// Add a public <see cref="MethodExpression"/> to the <see cref="TypeExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="body"/>.
        /// </summary>
        /// <param name="typeConfig">The <see cref="ITypeExpressionConfigurator"/> to configure.</param>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="body">
        /// The Expression from which to create the <see cref="MethodExpression"/>'s parameters and
        /// body.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public static MethodExpression AddMethod(
            this ITypeExpressionConfigurator typeConfig,
            string name,
            Expression body)
        {
            return typeConfig.AddMethod(name, body, cfg => { });
        }
    }
}