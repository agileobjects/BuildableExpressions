namespace AgileObjects.BuildableExpressions
{
    using System;
    using SourceCode;
    using SourceCode.Api;

    /// <summary>
    /// Provides buildable Expression factory methods.
    /// </summary>
    public static partial class BuildableExpression
    {
        /// <summary>
        /// Gets or sets the default namespace to which generated types will be added.
        /// </summary>
        public static string DefaultNamespace { get; set; }

        /// <summary>
        /// Creates a <see cref="SourceCodeExpression"/> representing a complete piece of source code.
        /// </summary>
        /// <param name="configuration">The configuration to use for the <see cref="SourceCodeExpression"/>.</param>
        /// <returns>A <see cref="SourceCodeExpression"/> representing a complete piece of source code.</returns>
        public static SourceCodeExpression SourceCode(
            Action<ISourceCodeExpressionConfigurator> configuration)
        {
            if (DefaultNamespace == null)
            {
                return new ConfiguredSourceCodeExpression(configuration);
            }

            return new ConfiguredSourceCodeExpression(cfg =>
            {
                cfg.SetNamespace(DefaultNamespace);
                configuration.Invoke(cfg);
            });
        }

        /// <summary>
        /// Creates a <see cref="SourceCodeExpression"/> for the given piece of
        /// <paramref name="sourceCode"/>. This overload supports 'magic-string' source code, as an
        /// alternative to building up a <see cref="SourceCodeExpression"/> using the configuration
        /// API. The resulting <see cref="SourceCodeExpression"/> will be unable to surface any
        /// <see cref="TypeExpression"/>s.
        /// </summary>
        /// <returns>A <see cref="SourceCodeExpression"/> for the given piece of <paramref name="sourceCode"/>.</returns>
        public static SourceCodeExpression SourceCode(string sourceCode)
            => LiteralSourceCodeExpression.Parse(sourceCode);
    }
}
