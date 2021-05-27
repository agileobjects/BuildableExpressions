namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents an attribute class in a piece of source code.
    /// </summary>
    public abstract class AttributeExpression : ClassExpressionBase, IType
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
        /// Gets the System.AttributeTargets defining to which code elements the
        /// <see cref="AttributeExpression"/> can be applied.
        /// </summary>
        public abstract AttributeTargets ValidOn { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="AttributeExpression"/> to be applied to
        /// the same code element multiple times. Defaults to false.
        /// </summary>
        public abstract bool AllowMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="AttributeExpression"/> is inherited by
        /// derived types and members. Defaults to true.
        /// </summary>
        public abstract bool Inherited { get; }

        /// <summary>
        /// Gets the <see cref="AttributeExpression"/> from which this 
        /// <see cref="AttributeExpression"/> derives. If this <see cref="AttributeExpression"/> 
        /// derives from System.Attribute, returns an <see cref="AttributeExpression"/> representing
        /// System.Attribute.
        /// </summary>
        public AttributeExpression BaseTypeExpression { get; protected set; }

        internal override ClassExpressionBase BaseTypeClassExpression => BaseTypeExpression;

        #region IType Members

        IType IType.BaseType => ClrTypeWrapper.Attribute;

        #endregion
    }
}