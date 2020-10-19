namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents the instance of an object to which the 'this' keyword relates in the current
    /// context.
    /// </summary>
    public sealed class ThisInstanceExpression : Expression
    {
        internal ThisInstanceExpression(ConcreteTypeExpression instance)
        {
            Instance = instance;
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1002) indicating the type of this
        /// <see cref="ThisInstanceExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.ThisInstance;

        /// <summary>
        /// Gets the type of this <see cref="ThisInstanceExpression"/>.
        /// </summary>
        public override Type Type => Instance.Type;

        /// <summary>
        /// Visits this <see cref="ThisInstanceExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="ThisInstanceExpression"/>.
        /// </param>
        /// <returns>This <see cref="ThisInstanceExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets the <see cref="ConcreteTypeExpression"/> representing the instance Type.
        /// </summary>
        public ConcreteTypeExpression Instance { get; }
    }
}