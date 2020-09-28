namespace AgileObjects.BuildableExpressions
{
    using System;
    using SourceCode;
    using SourceCode.Api;

    /// <summary>
    /// A factory class with which to build <see cref="SourceCodeExpression"/>s.
    /// </summary>
    public class SourceCodeFactory
    {
        /// <summary>
        /// Gets the default <see cref="SourceCodeFactory"/> instance.
        /// </summary>
        public static readonly SourceCodeFactory Default = new SourceCodeFactory();

        private readonly SourceCodeTranslationSettings _settings;

        private SourceCodeFactory()
        {
            _settings = SourceCodeTranslationSettings.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeFactory"/> class using the given
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        public SourceCodeFactory(
            Func<ISourceCodeTranslationSettings, ISourceCodeTranslationSettings> configuration)
        {
            _settings = SourceCodeTranslationSettings.Create();
            configuration.Invoke(_settings);
        }

        /// <summary>
        /// Creates a <see cref="SourceCodeExpression"/> representing a complete piece of source code.
        /// </summary>
        /// <returns>A <see cref="SourceCodeExpression"/> representing a complete piece of source code.</returns>
        public SourceCodeExpression CreateSourceCode() => CreateSourceCode(cfg => cfg);

        /// <summary>
        /// Creates a <see cref="SourceCodeExpression"/> representing a complete piece of source code.
        /// </summary>
        /// <param name="configuration">The configuration to use for the translation.</param>
        /// <returns>A <see cref="SourceCodeExpression"/> representing a complete piece of source code.</returns>
        public SourceCodeExpression CreateSourceCode(
            Func<ISourceCodeExpressionConfigurator, ISourceCodeExpressionConfigurator> configuration)
        {
            var sourceCode = new SourceCodeExpression(_settings);
            configuration.Invoke(sourceCode);

            return sourceCode;
        }
    }
}
