namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public abstract class ClassExpression : ConcreteTypeExpression
    {
        private ClassExpression _baseTypeExpression;
        private Type _baseType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="ClassExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="baseType">The base type from which the <see cref="ClassExpression"/> derives.</param>
        /// <param name="name">The name of the <see cref="ClassExpression"/>.</param>
        protected ClassExpression(
            SourceCodeExpression sourceCode,
            Type baseType,
            string name)
            : base(sourceCode, name)
        {
            _baseType = baseType;
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
        /// Gets the <see cref="ClassExpression"/> from which this <see cref="ClassExpression"/>
        /// derives. If no base type has been set, returns null.
        /// </summary>
        public ClassExpression BaseTypeExpression
        {
            get => _baseTypeExpression ??= GetBaseTypeExpressionOrNull();
            protected set => _baseTypeExpression = value;
        }

        #region BaseTypeExpression Creation

        private ClassExpression GetBaseTypeExpressionOrNull()
        {
            if (HasObjectBaseType)
            {
                return null;
            }

            var configuredBaseTypeExpression = (ClassExpression)SourceCode
                .TypeExpressions
                .FirstOrDefault(t => t.TypeAccessor == _baseType);

            return configuredBaseTypeExpression ??
                   new TypedClassExpression(SourceCode, _baseType);
        }

        #endregion

        /// <summary>
        /// Gets the base type from which this <see cref="ClassExpression"/> derives. If no base
        /// type has been set, returns typeof(System.Object).
        /// </summary>
        public Type BaseType
        {
            get => _baseType ??= BaseTypeExpression.Type;
            protected set => _baseType = value;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> has the default
        /// System.Object base type.
        /// </summary>
        protected bool HasObjectBaseType => _baseType == typeof(object);

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
        {
            get
            {
                if (!HasObjectBaseType)
                {
                    yield return _baseType;
                }

                foreach (var @interface in base.ImplementedTypes)
                {
                    yield return @interface;
                }
            }
        }

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}