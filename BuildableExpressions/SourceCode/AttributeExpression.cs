namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;

    /// <summary>
    /// Represents an attribute class in a piece of source code.
    /// </summary>
    public abstract class AttributeExpression : ClassExpressionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeExpression"/> class for the given
        /// <paramref name="attributeType"/>.
        /// </summary>
        /// <param name="attributeType">The attribute Type represented by the <see cref="AttributeExpression"/>.</param>
        protected AttributeExpression(Type attributeType)
            : base(attributeType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="AttributeExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="AttributeExpression"/>.</param>
        protected AttributeExpression(
            SourceCodeExpression sourceCode,
            string name)
            : base(sourceCode, name)
        {
        }

        /// <summary>
        /// Gets the <see cref="AttributeExpression"/> from which this 
        /// <see cref="AttributeExpression"/> derives. If this <see cref="AttributeExpression"/> 
        /// derives from System.Attribute, returns an <see cref="AttributeExpression"/> representing
        /// System.Attribute.
        /// </summary>
        public AttributeExpression BaseTypeExpression { get; protected set; }

        /// <summary>
        /// Gets the <see cref="AttributeExpression"/> from which this 
        /// <see cref="AttributeExpression"/> If this <see cref="AttributeExpression"/> derives from
        /// System.Attribute, returns an <see cref="AttributeExpression"/> representing
        /// System.Attribute.
        /// </summary>
        protected override ClassExpressionBase BaseTypeClassExpression => BaseTypeExpression;
    }
}