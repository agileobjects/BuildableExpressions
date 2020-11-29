namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a type or method generic parameter.
    /// </summary>
    public abstract class GenericParameterExpression :
        TypeExpression,
        IGenericParameter,
        ICustomTranslationExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterExpression"/> class for the
        /// given <paramref name="parameterType"/>.
        /// </summary>
        /// <param name="parameterType">
        /// The Type represented by the <see cref="GenericParameterExpression"/>.
        /// </param>
        protected GenericParameterExpression(Type parameterType)
            : base(parameterType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="GenericParameterExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        protected GenericParameterExpression(
            SourceCodeExpression sourceCode,
            string name)
            : base(sourceCode, name)
        {
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        #region IGenericParameter Members

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained.
        /// </summary>
        public abstract bool HasConstraints { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained to reference types.
        /// </summary>
        public abstract bool HasClassConstraint { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained to value types.
        /// </summary>
        public abstract bool HasStructConstraint { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained to types with a public, parameterless constructor.
        /// </summary>
        public abstract bool HasNewableConstraint { get; }

        #endregion

        /// <summary>
        /// Gets the <see cref="IType"/>s describing the Types to which this
        /// <see cref="GenericParameterExpression"/> is constrained, if any.
        /// </summary>
        public abstract ReadOnlyCollection<IType> TypeConstraints { get; }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => GetTranslation(context);

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
            => context.GetTranslationFor((IType)this);
    }
}