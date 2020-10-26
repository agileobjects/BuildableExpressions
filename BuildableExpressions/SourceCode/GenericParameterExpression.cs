namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a type or method generic argument.
    /// </summary>
    public abstract class GenericParameterExpression :
        Expression,
        IGenericArgument,
        ICustomTranslationExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterExpression"/> class.
        /// </summary>
        /// <param name="name"></param>
        protected GenericParameterExpression(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        /// <summary>
        /// Visits this <see cref="GenericParameterExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="GenericParameterExpression"/>.
        /// </param>
        /// <returns>This <see cref="GenericParameterExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets the name of this <see cref="GenericParameterExpression"/>
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public abstract bool IsClosed { get; }

        #region IGenericArgument Members

        bool IGenericArgument.HasConstraints => HasConstraints;

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained.
        /// </summary>
        protected abstract bool HasConstraints { get; }

        bool IGenericArgument.HasClassConstraint => HasClassConstraint;

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained to reference types.
        /// </summary>
        protected abstract bool HasClassConstraint { get; }

        bool IGenericArgument.HasStructConstraint => HasStructConstraint;

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained to value types.
        /// </summary>
        protected abstract bool HasStructConstraint { get; }

        bool IGenericArgument.HasNewableConstraint => HasNewableConstraint;

        /// <summary>
        /// Gets a value indicating whether this <see cref="GenericParameterExpression"/> is
        /// constrained to types with a public, parameterless constructor.
        /// </summary>
        protected abstract bool HasNewableConstraint { get; }

        ReadOnlyCollection<Type> IGenericArgument.TypeConstraints => TypeConstraints;

        /// <summary>
        /// Gets this <see cref="GenericParameterExpression"/>'s collection of Type constraints, if
        /// any.
        /// </summary>
        protected abstract ReadOnlyCollection<Type> TypeConstraints { get; }

        #endregion

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => context.GetTranslationFor(Type);
    }
}