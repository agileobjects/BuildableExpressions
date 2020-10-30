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

        internal ClassExpression(
            SourceCodeExpression sourceCode,
            Type baseType,
            string name)
            : base(sourceCode, name)
        {
            BaseType = baseType;
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
            => _baseTypeExpression ??= GetBaseTypeExpressionOrNull();

        #region BaseTypeExpression Creation

        private ClassExpression GetBaseTypeExpressionOrNull()
        {
            if (HasObjectBaseType)
            {
                return null;
            }

            var configuredBaseTypeExpression = (ClassExpression)SourceCode
                .TypeExpressions
                .FirstOrDefault(t => t.TypeAccessor == BaseType);

            if (configuredBaseTypeExpression != null)
            {
                return configuredBaseTypeExpression;
            }

            return new TypedClassExpression(SourceCode, BaseType);
        }

        #endregion

        /// <summary>
        /// Gets the base type from which this <see cref="ClassExpression"/> derives. If no base
        /// type has been set, returns typeof(System.Object).
        /// </summary>
        public Type BaseType { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> has the default
        /// System.Object base type.
        /// </summary>
        protected bool HasObjectBaseType => BaseType == typeof(object);

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
                    yield return BaseType;
                }

                foreach (var @interface in InterfaceTypes)
                {
                    yield return @interface;
                }
            }
        }

        internal override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}