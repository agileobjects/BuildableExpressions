namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a chained call from a class or struct constructor to base or sibling constructor
    /// in a piece of source code.
    /// </summary>
    public class ChainedConstructorCallExpression :
        Expression,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        internal ChainedConstructorCallExpression(
            ConstructorExpression callingConstructor,
            ConstructorExpression targetConstructor,
            IList<Expression> arguments)
        {
            CallingConstructor = callingConstructor;
            TargetConstructor = targetConstructor;
            Arguments = arguments.ToReadOnlyCollection();
        }

        /// <summary>
        /// Gets the ExpressionType value (6 - Call) indicating the type of this
        /// <see cref="ChainedConstructorCallExpression"/>.
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Call;

        /// <summary>
        /// Gets the type of this <see cref="ChainedConstructorCallExpression"/>.
        /// </summary>
        public override Type Type => CallingConstructor.Type;

        /// <summary>
        /// Visits this <see cref="ChainedConstructorCallExpression"/>'s
        /// <see cref="Arguments"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="ChainedConstructorCallExpression"/>.
        /// </param>
        /// <returns>This <see cref="ChainedConstructorCallExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(Arguments);
            return this;
        }

        /// <summary>
        /// Gets the <see cref="ConstructorExpression"/> representing the calling constructor.
        /// </summary>
        public ConstructorExpression CallingConstructor { get; }

        /// <summary>
        /// Gets the <see cref="ConstructorExpression"/> representing the constructor being called.
        /// </summary>
        public ConstructorExpression TargetConstructor { get; }

        /// <summary>
        /// Gets the Expression(s) being passed to the <see cref="TargetConstructor"/>, if any.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments { get; }

        #region ICustomAnalysableExpression Members

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions => Arguments;

        #endregion

        #region ICustomTranslationExpression Members

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new ChainedConstructorCallTranslation(this, context);

        #endregion
    }
}