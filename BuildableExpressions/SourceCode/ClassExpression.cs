namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public abstract class ClassExpression : ConcreteTypeExpression, IType
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
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is abstract.
        /// </summary>
        public bool IsSealed { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> has the default
        /// System.Object base type.
        /// </summary>
        public bool HasObjectBaseType => BaseTypeExpression == null;

        /// <summary>
        /// Gets the <see cref="ClassExpression"/> from which this <see cref="ClassExpression"/>
        /// derives. If no base type has been set, returns null.
        /// </summary>
        public ClassExpression BaseTypeExpression { get; protected set; }

        /// <summary>
        /// Gets the base type from which this <see cref="ClassExpression"/> derives. If no base
        /// type has been set, returns typeof(System.Object).
        /// </summary>
        public Type BaseType => BaseTypeExpression?.Type ?? typeof(object);

        /// <summary>
        /// Gets the non-object <see cref="BaseTypeExpression"/> and <see cref="InterfaceExpression"/>s
        /// implemented by this <see cref="ClassExpression"/>.
        /// </summary>
        public override IEnumerable<TypeExpression> ImplementedTypeExpressions
        {
            get
            {
                if (!HasObjectBaseType)
                {
                    yield return BaseTypeExpression;
                }

                foreach (var @interface in base.ImplementedTypeExpressions)
                {
                    yield return @interface;
                }
            }
        }

        /// <summary>
        /// Gets the non-object base type and interfaces types implemented by this
        /// <see cref="ClassExpression"/>.
        /// </summary>
        public override IEnumerable<Type> ImplementedTypes
            => ImplementedTypeExpressions.Project(t => t.Type);

        #region IType Members

        IType IType.BaseType => BaseTypeExpression;

        internal override bool IsClass => true;

        bool IType.IsAbstract => IsAbstract;

        bool IType.IsSealed => IsSealed;

        #endregion
    }
}