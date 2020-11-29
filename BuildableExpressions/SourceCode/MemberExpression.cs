namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Linq.Expressions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Abstract base class for Expressions describing a type member.
    /// </summary>
    public abstract class MemberExpression :
        MemberExpressionBase,
        ICustomTranslationExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberExpression"/> class.
        /// </summary>
        /// <param name="declaringTypeExpression">
        /// This <see cref="MemberExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </param>
        /// <param name="name">The name of this <see cref="MemberExpression"/>.</param>
        protected MemberExpression(
            TypeExpression declaringTypeExpression,
            string name)
            : base(name)
        {
            DeclaringTypeExpression = declaringTypeExpression;
        }

        /// <summary>
        /// Visits this <see cref="MemberExpression"/>'s Body.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="MemberExpression"/>.</param>
        /// <returns>This <see cref="MemberExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
            => this;

        /// <summary>
        /// Gets this <see cref="MemberExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </summary>
        public TypeExpression DeclaringTypeExpression { get; }

        /// <summary>
        /// Gets this <see cref="MemberExpression"/>'s parent Type.
        /// </summary>
        public override IType DeclaringType => DeclaringTypeExpression;

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="MemberExpression"/>,
        /// or null if no summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Set the summary documentation of this <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MemberExpression"/>.
        /// </param>
        protected void SetSummary(CommentExpression summary)
            => Summary = summary;

        #region Translation

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            return DeclaringTypeExpression.SourceCode.IsComplete
                ? GetFullTranslation(context) : GetTransientTranslation(context);
        }

        /// <summary>
        /// When overridden in a derived class, gets an <see cref="ITranslation"/> with which to
        /// translate this <see cref="MemberExpression"/>. This method is used if a
        /// <see cref="TypeExpression"/> needs to generate its Type object once configuration is
        /// complete.
        /// </summary>
        /// <param name="context">The ITranslationContext describing the current translation.</param>
        /// <returns>
        /// An <see cref="ITranslation"/> with which to fully translate this
        /// <see cref="MemberExpression"/>.
        /// </returns>
        protected abstract ITranslation GetFullTranslation(ITranslationContext context);

        /// <summary>
        /// When overridden in a derived class, gets an <see cref="ITranslation"/> with which to
        /// minimally translate this <see cref="MemberExpression"/>. This method is used if a
        /// <see cref="TypeExpression"/> needs to generate its Type object during configuration.
        /// </summary>
        /// <param name="context">The ITranslationContext describing the current translation.</param>
        /// <returns>
        /// An <see cref="ITranslation"/> with which to minimally translate this
        /// <see cref="MemberExpression"/>.
        /// </returns>
        protected abstract ITranslation GetTransientTranslation(ITranslationContext context);

        #endregion
    }
}