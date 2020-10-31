namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;

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
        public override Type DeclaringType => DeclaringTypeExpression.Type;

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="MemberExpression"/>,
        /// or null if no summary has been set.
        /// </summary>
        public CommentExpression Summary { get; protected set; }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => GetTranslation(context);

        /// <summary>
        /// When overridden in a derived class, gets an <see cref="ITranslation"/> with which to
        /// translate this <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="context">The ITranslationContext describing the current translation.</param>
        /// <returns>
        /// An <see cref="ITranslation"/> with which to translate this
        /// <see cref="MemberExpression"/>.
        /// </returns>
        protected abstract ITranslation GetTranslation(ITranslationContext context);
    }
}