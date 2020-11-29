namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a type or method open generic parameter.
    /// </summary>
    public abstract class OpenGenericParameterExpression : GenericParameterExpression, IType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGenericParameterExpression"/> class
        /// for the given <paramref name="parameterType"/>.
        /// </summary>
        /// <param name="parameterType">
        /// The Type represented by the <see cref="OpenGenericParameterExpression"/>.
        /// </param>
        protected OpenGenericParameterExpression(Type parameterType)
            : base(parameterType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGenericParameterExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="OpenGenericParameterExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="OpenGenericParameterExpression"/>.</param>
        protected OpenGenericParameterExpression(
            SourceCodeExpression sourceCode,
            string name)
            : base(sourceCode, name)
        {
        }

        #region IType Members

        bool IType.IsGenericParameter => true;

        #endregion
    }
}