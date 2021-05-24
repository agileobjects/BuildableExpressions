namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Generics;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public abstract class ClassExpression : ClassExpressionBase, IClosableTypeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpression"/> class for the given
        /// <paramref name="classType"/>.
        /// </summary>
        /// <param name="classType">The Type represented by the <see cref="ClassExpression"/>.</param>
        protected ClassExpression(Type classType)
            : base(classType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="ClassExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="ClassExpression"/>.</param>
        protected ClassExpression(
            SourceCodeExpression sourceCode,
            string name)
            : base(sourceCode, name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is static.
        /// </summary>
        public bool IsStatic { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> has the default
        /// System.Object base type.
        /// </summary>
        public bool HasObjectBaseType => BaseTypeExpression == null;

        /// <summary>
        /// Gets the <see cref="ClassExpression"/> from which this <see cref="ClassExpression"/>
        /// derives. If this <see cref="ClassExpression"/> derives from System.Object, returns null.
        /// </summary>
        public ClassExpression BaseTypeExpression { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ClassExpression"/> from which this <see cref="ClassExpression"/>
        /// derives. If this <see cref="ClassExpression"/> derives from System.Object, returns null.
        /// </summary>
        protected override ClassExpressionBase BaseTypeClassExpression => BaseTypeExpression;

        #region IClosableTypeExpression Members

        IClosableTypeExpression IClosableTypeExpression.Close(
            GenericParameterExpression genericParameter,
            TypeExpression closedTypeExpression)
        {
            var closedClass = CreateInstance();
            Close(closedClass, genericParameter, closedTypeExpression);
            return closedClass;
        }

        /// <summary>
        /// Creates a new instance of this <see cref="ClassExpression"/>.
        /// </summary>
        /// <returns>A newly-created instance of this <see cref="ClassExpression"/>.</returns>
        protected abstract ClassExpression CreateInstance();

        #endregion
    }
}