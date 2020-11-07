namespace AgileObjects.BuildableExpressions.SourceCode
{
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents an interface in a piece of source code.
    /// </summary>
    public abstract class InterfaceExpression : TypeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="InterfaceExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="InterfaceExpression"/>.</param>
        protected InterfaceExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
            => new InterfaceTranslation(this, context);
    }
}