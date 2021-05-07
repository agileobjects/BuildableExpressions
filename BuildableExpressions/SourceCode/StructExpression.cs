namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a struct in a piece of source code.
    /// </summary>
    public abstract class StructExpression : ConcreteTypeExpression, IType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructExpression"/> class for the given
        /// <paramref name="structType"/>.
        /// </summary>
        /// <param name="structType">The Type represented by the <see cref="StructExpression"/>.</param>
        protected StructExpression(Type structType)
            : base(structType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="StructExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="StructExpression"/>.</param>
        protected StructExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IType Members

        IType IType.BaseType => ClrTypeWrapper.ValueType;

        #endregion
    }
}