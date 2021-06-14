namespace AgileObjects.BuildableExpressions
{
    using System;
    using SourceCode;
    using SourceCode.Operators;

    /// <summary>
    /// Provides buildable Expression factory methods.
    /// </summary>
    public static partial class BuildableExpression
    {
        /// <summary>
        /// Create a <see cref="TypeOfOperatorExpression"/> that represents use of the typeof
        /// operator on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The Type to which the typeof operator is being applied.</param>
        /// <returns>
        /// A <see cref="TypeOfOperatorExpression"/> representing use of the typeof operator on
        /// the given <paramref name="type"/>
        /// </returns>
        public static TypeOfOperatorExpression TypeOf(Type type)
            => TypeOf(TypeExpressionFactory.Create(type));

        /// <summary>
        /// Create a <see cref="TypeOfOperatorExpression"/> that represents use of the typeof
        /// operator on the given <paramref name="typeExpression"/>.
        /// </summary>
        /// <param name="typeExpression">
        /// The <see cref="TypeExpression"/> to which the typeof operator is being applied.
        /// </param>
        /// <returns>
        /// A <see cref="TypeOfOperatorExpression"/> representing use of the typeof operator on
        /// the given <paramref name="typeExpression"/>
        /// </returns>
        public static TypeOfOperatorExpression TypeOf(TypeExpression typeExpression)
            => new(typeExpression);

        /// <summary>
        /// Create a <see cref="NameOfOperatorExpression"/> that represents use of the nameof
        /// operator on the given <paramref name="typeExpression"/>.
        /// </summary>
        /// <param name="typeExpression">
        /// The <see cref="TypeExpression"/> to which the nameof operator is being applied.
        /// </param>
        /// <returns>
        /// A <see cref="NameOfOperatorExpression"/> representing use of the nameof operator on
        /// the given <paramref name="typeExpression"/>
        /// </returns>
        public static NameOfOperatorExpression NameOf(TypeExpression typeExpression)
            => new NameOfOperatorExpression(typeExpression);
    }
}
