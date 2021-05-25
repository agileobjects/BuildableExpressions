namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public abstract class ClassExpressionBase : ConcreteTypeExpression, IType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpressionBase"/> class for the given
        /// <paramref name="classType"/>.
        /// </summary>
        /// <param name="classType">The Type represented by the <see cref="ClassExpressionBase"/>.</param>
        protected ClassExpressionBase(Type classType)
            : base(classType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpressionBase"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="ClassExpressionBase"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="ClassExpressionBase"/>.</param>
        protected ClassExpressionBase(
            SourceCodeExpression sourceCode,
            string name)
            : base(sourceCode, name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpressionBase"/> is static.
        /// </summary>
        public bool IsStatic { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpressionBase"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpressionBase"/> is sealed.
        /// </summary>
        public bool IsSealed { get; protected set; }

        internal abstract ClassExpressionBase BaseTypeClassExpression { get; }

        /// <summary>
        /// Gets the base type from which this <see cref="ClassExpressionBase"/> derives. If this
        /// <see cref="ClassExpressionBase"/> derives from System.Object, returns
        /// typeof(System.Object).
        /// </summary>
        public Type BaseType => BaseTypeClassExpression?.Type ?? typeof(object);

        /// <summary>
        /// Gets the non-object base <see cref="ClassExpressionBase"/> and 
        /// <see cref="InterfaceExpression"/>s implemented by this <see cref="ClassExpressionBase"/>.
        /// </summary>
        public override IEnumerable<TypeExpression> ImplementedTypeExpressions
        {
            get
            {
                if (BaseTypeClassExpression != null)
                {
                    yield return BaseTypeClassExpression;
                }

                foreach (var @interface in base.ImplementedTypeExpressions)
                {
                    yield return @interface;
                }
            }
        }

        /// <summary>
        /// Gets the non-object base type and interfaces types implemented by this
        /// <see cref="ClassExpressionBase"/>.
        /// </summary>
        public override IEnumerable<Type> ImplementedTypes
            => ImplementedTypeExpressions.Project(t => t.Type);

        #region IType Members

        IType IType.BaseType => BaseTypeClassExpression;

        internal override bool IsClass => true;

        bool IType.IsAbstract => IsAbstract;

        bool IType.IsSealed => IsSealed;

        #endregion
    }
}