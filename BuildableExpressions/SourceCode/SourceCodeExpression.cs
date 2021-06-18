namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using ReadableExpressions;

    /// <summary>
    /// Represents a piece of complete source code.
    /// </summary>
    public abstract class SourceCodeExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeExpression"/> class.
        /// </summary>
        /// <param name="namespace">
        /// The namespace to which the source code represented by this
        /// <see cref="SourceCodeExpression"/> belongs.
        /// </param>
        protected SourceCodeExpression(string @namespace)
        {
            Namespace = @namespace;
        }

        internal abstract bool IsComplete { get; }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1000) indicating the type of this
        /// <see cref="SourceCodeExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.SourceCode;

        /// <summary>
        /// Gets the type of this <see cref="SourceCodeExpression"/> - typeof(void).
        /// </summary>
        public override Type Type => typeof(void);

        /// <summary>
        /// Visits each of this <see cref="SourceCodeExpression"/>'s <see cref="TypeExpressions"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="SourceCodeExpression"/>'s
        /// <see cref="TypeExpressions"/>.
        /// </param>
        /// <returns>This <see cref="SourceCodeExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            foreach (var @class in TypeExpressions)
            {
                visitor.Visit(@class);
            }

            return this;
        }

        /// <summary>
        /// Gets the Assemblies referenced by this <see cref="SourceCodeExpression"/>.
        /// </summary>
        public abstract ReadOnlyCollection<Assembly> ReferencedAssemblies { get; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> containing the file header to use in the source
        /// code generated from this <see cref="SourceCodeExpression"/>, if a header has been set.
        /// </summary>
        public CommentExpression Header { get; protected set; }

        /// <summary>
        /// Gets the namespace to which the source code represented by this
        /// <see cref="SourceCodeExpression"/> belongs.
        /// </summary>
        public string Namespace { get; protected set; }

        /// <summary>
        /// Gets the <see cref="TypeExpression"/>s which describe the types defined by this
        /// <see cref="SourceCodeExpression"/>.
        /// </summary>
        public abstract ReadOnlyCollection<TypeExpression> TypeExpressions { get; }

        /// <summary>
        /// Gets the name of the Type contained in this <see cref="SourceCodeExpression"/>. If
        /// multiple types exist, returns the name of the first.
        /// </summary>
        public abstract string TypeName { get; }

        /// <summary>
        /// Translates this <see cref="SourceCodeExpression"/> to a complete source-code string,
        /// formatted as one or more types with one or more methods in a namespace.
        /// </summary>
        /// <returns>
        /// The translated <see cref="SourceCodeExpression"/>, formatted as one or more types with
        /// one or more methods in a namespace.
        /// </returns>
        public abstract string ToSourceCodeString();
    }
}
