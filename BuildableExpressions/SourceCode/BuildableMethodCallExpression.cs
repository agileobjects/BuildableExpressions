namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Extensions;

    /// <summary>
    /// Represents a call to a generated <see cref="MethodExpression"/>.
    /// </summary>
    public sealed class BuildableMethodCallExpression : Expression
    {
        internal BuildableMethodCallExpression(
            Expression @object,
            MethodExpression method,
            IList<Expression> arguments)
        {
            Object = @object;
            Method = method;
            Arguments = arguments.ToReadOnlyCollection();
        }

        /// <summary>
        /// Gets the ExpressionType value (6 - Call) indicating the type of this
        /// <see cref="BuildableMethodCallExpression"/>.
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Call;

        /// <summary>
        /// Gets the Type of this <see cref="BuildableMethodCallExpression"/>.
        /// </summary>
        public override Type Type => Method.Type;

        /// <summary>
        /// Visits each of this <see cref="BuildableMethodCallExpression"/>'s <see cref="Method"/>
        /// and <see cref="Arguments"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="BuildableMethodCallExpression"/>'s
        /// <see cref="Method"/> and <see cref="Arguments"/>.
        /// </param>
        /// <returns>This <see cref="BuildableMethodCallExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(Method);

            foreach (var argument in Arguments)
            {
                visitor.Visit(argument);
            }

            return this;
        }

        /// <summary>
        /// The <see cref="MethodExpression"/> representing the method being called.
        /// </summary>
        public MethodExpression Method { get; }

        /// <summary>
        /// Gets the Expression representing the instance on which the <see cref="Method"/> is being
        /// called, or null if the <see cref="Method"/> is static.
        /// </summary>
        public Expression Object { get; }

        /// <summary>
        /// Expressions representing the method call arguments.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments { get; }
    }
}
