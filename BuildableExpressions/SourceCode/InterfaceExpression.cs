namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Generics;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents an interface in a piece of source code.
    /// </summary>
    public abstract class InterfaceExpression : TypeExpression, IClosableTypeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceExpression"/> class for the given
        /// <paramref name="interfaceType"/>.
        /// </summary>
        /// <param name="interfaceType">The Type represented by the <see cref="InterfaceExpression"/>.</param>
        protected InterfaceExpression(Type interfaceType)
            : base(interfaceType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="InterfaceExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="InterfaceExpression"/>.</param>
        internal InterfaceExpression(ConfiguredSourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IType Members

        internal override bool IsInterface => true;

        bool IType.IsAbstract => true;

        #endregion

        #region IClosableTypeExpression Members
        
        IClosableTypeExpression IClosableTypeExpression.Close(
            GenericParameterExpression genericParameter, 
            TypeExpression closedTypeExpression)
        {
            var closedInterface = CreateInstance();
            Close(closedInterface, genericParameter, closedTypeExpression);
            return closedInterface;
        }

        /// <summary>
        /// Creates a new instance of this <see cref="InterfaceExpression"/>.
        /// </summary>
        /// <returns>A newly-created instance of this <see cref="InterfaceExpression"/>.</returns>
        protected abstract InterfaceExpression CreateInstance();

        #endregion
    }
}