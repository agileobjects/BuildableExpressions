namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using BuildableExpressions.Extensions;
    using Extensions;
    using Generics;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    /// <summary>
    /// Represents a class or struct constructor in a piece of source code.
    /// </summary>
    public abstract class ConstructorExpression :
        MethodExpressionBase,
        IConstructor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorExpression"/> class.
        /// </summary>
        /// <param name="declaringTypeExpression">
        /// This <see cref="ConstructorExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </param>
        protected ConstructorExpression(TypeExpression declaringTypeExpression)
            : base(declaringTypeExpression, string.Empty)
        {
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1005) indicating the type of this
        /// <see cref="ConstructorExpression"/> as an ExpressionType.
        /// </summary>
        protected override SourceCodeExpressionType SourceCodeNodeType
            => SourceCodeExpressionType.Constructor;

        internal bool HasChainedConstructorCall => ChainedConstructorCall != null;

        /// <summary>
        /// Gets the <see cref="ChainedConstructorCallExpression"/> representing the chained call
        /// to base or sibling <see cref="ConstructorExpression"/>, if one exists.
        /// </summary>
        public ChainedConstructorCallExpression ChainedConstructorCall { get; private set; }

        /// <summary>
        /// Set this <see cref="ConstructorExpression"/>'s ChainedConstructorCall.
        /// </summary>
        /// <param name="call">The <see cref="ChainedConstructorCallExpression"/> to use.</param>
        protected void SetChainedConstructorCall(ChainedConstructorCallExpression call)
            => ChainedConstructorCall = call;

        /// <inheritdoc />
        public override string Name => DeclaringTypeExpression.Name;

        internal void SetBody(Expression body) => SetBody(body, typeof(void));

        /// <summary>
        /// Gets the ConstructorInfo for this <see cref="ConstructorExpression"/>, which is lazily,
        /// dynamically generated using this constructor's definition.
        /// </summary>
        public abstract ConstructorInfo ConstructorInfo { get; }

        #region IMember Members

        IType IMember.DeclaringType => DeclaringTypeExpression;

        IType IMember.Type => DeclaringTypeExpression;

        #endregion

        #region IComplexMember

        /// <inheritdoc />
        public override bool IsOverride => false;

        #endregion

        #region IMethod Members

        /// <inheritdoc />
        public override bool IsGeneric => false;

        /// <inheritdoc />
        public override ReadOnlyCollection<GenericParameterExpression> GenericParameters
            => Enumerable<GenericParameterExpression>.EmptyReadOnlyCollection;

        internal override IList<GenericParameterExpression> GenericParametersAccessor
            => Enumerable<GenericParameterExpression>.EmptyArray;

        /// <inheritdoc />
        protected override ReadOnlyCollection<IGenericParameter> GetGenericArguments()
            => Enumerable<IGenericParameter>.EmptyReadOnlyCollection;

        /// <inheritdoc />
        protected override IType GetReturnType() => DeclaringTypeExpression;

        #endregion

        /// <inheritdoc />
        protected override string GetSignature(bool includeTypeName)
        {
            var parameters = string.Join(
                ", ",
                ParametersAccessor?
                    .Project(p => p.Type.GetFriendlyName()) ??
                Array.Empty<string>());

            return $"{DeclaringTypeExpression.GetFriendlyName()}({parameters})";
        }

        #region Validation

        /// <inheritdoc />
        protected override string MethodTypeName => "constructor";

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new ConstructorTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => new TransientConstructorTranslation(this, context);

        #endregion

        /// <inheritdoc />
        public override string ToString() => GetSignature(includeTypeName: true);
    }
}