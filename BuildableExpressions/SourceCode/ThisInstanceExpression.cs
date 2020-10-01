namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Linq.Expressions;

    /// <summary>
    /// Represents the instance of an object to which the 'this' keyword relates in the current
    /// context.
    /// </summary>
    public sealed class ThisInstanceExpression : Expression
    {
        internal ThisInstanceExpression(ClassExpression @class)
        {
            Class = @class;
        }

        /// <summary>
        /// Gets the <see cref="ClassExpression"/> representing the instance Type.
        /// </summary>
        public ClassExpression Class { get; }
    }
}