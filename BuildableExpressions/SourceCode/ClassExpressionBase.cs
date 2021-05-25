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

        #region Validation

        /// <summary>
        /// Throws an InvalidOperationException due to an attempt to set the given
        /// <paramref name="attemptedBaseType"/> to this <see cref="ClassExpressionBase"/>'s base
        /// type, when a base type has already been set. 
        /// </summary>
        /// <param name="attemptedBaseType">
        /// The IType describing the base type which has been attempted to be set.
        /// </param>
        /// <param name="typeName">The name of the type of Type on which a base type has been set.</param>
        protected void ThrowBaseTypeAlreadySet(
            IType attemptedBaseType,
            string typeName)
        {
            throw new InvalidOperationException(
                $"Unable to set {typeName} base type to '{attemptedBaseType.Name}' " +
                $"as it has already been set to '{BaseTypeClassExpression.Name}'");
        }

        /// <summary>
        /// Throws an InvalidOperationException if the given <paramref name="attemptedBaseType"/>
        /// is not a valid base type for this <see cref="ClassExpressionBase"/>'s. 
        /// </summary>
        /// <param name="attemptedBaseType">
        /// The IType describing the base type which has been attempted to be set.
        /// </param>
        protected void ThrowIfInvalidBaseType(IType attemptedBaseType)
        {
            if (attemptedBaseType.IsSealed)
            {
                ThrowInvalidBaseType(attemptedBaseType);
            }
        }

        /// <summary>
        /// Throws an InvalidOperationException due to an attempt to set the given, invalid
        /// <paramref name="attemptedBaseType"/> to this <see cref="ClassExpressionBase"/>'s base
        /// type. 
        /// </summary>
        /// <param name="attemptedBaseType">
        /// The IType describing the base type which has been attempted to be set.
        /// </param>
        protected static void ThrowInvalidBaseType(IType attemptedBaseType)
        {
            throw new InvalidOperationException(
                $"Type '{attemptedBaseType.Name}' is not a valid base type.");
        }

        /// <summary>
        /// Throws an InvalidOperationException if this <see cref="ClassExpressionBase"/> is
        /// abstract, due to the given <paramref name="conflictingModifier"/> having been set.
        /// </summary>
        /// <param name="conflictingModifier">
        /// The modifier which would conflict with this <see cref="ClassExpressionBase"/> being
        /// static.
        /// </param>
        protected void ThrowIfAbstract(string conflictingModifier)
        {
            if (IsAbstract)
            {
                ThrowModifierConflict("abstract", conflictingModifier);
            }
        }

        /// <summary>
        /// Throws an InvalidOperationException if this <see cref="ClassExpressionBase"/> is
        /// sealed, due to the given <paramref name="conflictingModifier"/> having been set.
        /// </summary>
        /// <param name="conflictingModifier">
        /// The modifier which would conflict with this <see cref="ClassExpressionBase"/> being
        /// sealed.
        /// </param>
        protected void ThrowIfSealed(string conflictingModifier)
        {
            if (IsSealed)
            {
                ThrowModifierConflict("sealed", conflictingModifier);
            }
        }

        /// <summary>
        /// Throws an InvalidOperationException due to the attempted setting of the given
        /// <paramref name="conflictingModifier"/>, which conflicts with the existing given
        /// <paramref name="modifier"/>.
        /// </summary>
        /// <param name="modifier">
        /// The existing modifier with which the <paramref name="conflictingModifier"/> conflicts.
        /// </param>
        /// <param name="conflictingModifier">
        /// The modifier the attempted setting of which conflicted with the existing given
        /// <paramref name="modifier"/>.
        /// </param>
        protected void ThrowModifierConflict(string modifier, string conflictingModifier)
        {
            throw new InvalidOperationException(
                $"Type '{Name}' cannot be both {modifier} and {conflictingModifier}.");
        }

        #endregion
    }
}