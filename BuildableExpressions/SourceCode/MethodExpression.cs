namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Base class representing a method in a type in a piece of source code.
    /// </summary>
    public abstract class MethodExpression :
        MethodExpressionBase,
        IConcreteTypeMethodExpressionConfigurator,
        IMethodBase,
        IEquatable<MethodExpression>
    {
        private List<GenericParameterExpression> _genericParameters;
        private ReadOnlyCollection<GenericParameterExpression> _readOnlyGenericParameters;
        private ReadOnlyCollection<IGenericParameter> _readOnlyIGenericParameters;
        private MethodInfo _methodInfo;
        private string _name;
        private IType _returnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExpression"/> class.
        /// </summary>
        /// <param name="declaringTypeExpression">
        /// This <see cref="MethodExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </param>
        /// <param name="name">The name of this <see cref="MethodExpression"/>.</param>
        protected MethodExpression(TypeExpression declaringTypeExpression, string name)
            : base(declaringTypeExpression, name)
        {
            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1009) indicating the type of this
        /// <see cref="MethodExpression"/> as an ExpressionType.
        /// </summary>
        protected override SourceCodeExpressionType SourceCodeNodeType
            => SourceCodeExpressionType.Method;

        internal abstract bool HasGeneratedName { get; }

        /// <inheritdoc cref="MemberExpression" />
        public override string Name => _name;

        internal void SetName(string name) => _name = name;

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is generic.
        /// </summary>
        public override bool IsGeneric => _genericParameters?.Any() == true;

        /// <inheritdoc />
        public override ReadOnlyCollection<GenericParameterExpression> GenericParameters
        {
            get
            {
                return _readOnlyGenericParameters ??= IsGeneric
                    ? _genericParameters.ToReadOnlyCollection()
                    : Enumerable<GenericParameterExpression>.EmptyReadOnlyCollection;
            }
        }

        internal override IList<GenericParameterExpression> GenericParametersAccessor
            => _genericParameters;

        /// <summary>
        /// Gets the MethodInfo for this <see cref="MethodExpression"/>, which is lazily, dynamically
        /// generated using this method's definition.
        /// </summary>
        public virtual MethodInfo MethodInfo => _methodInfo ??= CreateMethodInfo();

        #region MethodInfo Creation

        private MethodInfo CreateMethodInfo()
        {
            var parameterTypes =
                ParametersAccessor?.ProjectToArray(p => p.Type) ??
                Type.EmptyTypes;

            var declaringType = DeclaringType.AsType();

            var method = Visibility == Public
                ? IsStatic
                    ? declaringType.GetPublicStaticMethod(Name, parameterTypes)
                    : declaringType.GetPublicInstanceMethod(Name, parameterTypes)
                : IsStatic
                    ? declaringType.GetNonPublicStaticMethod(Name, parameterTypes)
                    : declaringType.GetNonPublicInstanceMethod(Name, parameterTypes);

            return method;
        }

        #endregion

        #region IGenericParameterConfigurator Members

        GenericParameterExpression IGenericParameterConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            ThrowIfDuplicateGenericParameterName(name);
            ThrowIfTypeGenericParameterNameClash(name);

            var parameter = new ConfiguredGenericParameterExpression(
                DeclaringTypeExpression.SourceCode,
                name,
                configuration);

            _genericParameters ??= new List<GenericParameterExpression>();
            _genericParameters.Add(parameter);
            _readOnlyGenericParameters = null;
            _readOnlyIGenericParameters = null;
            return parameter;
        }

        private void ThrowIfDuplicateGenericParameterName(string name)
        {
            if (!IsGeneric)
            {
                return;
            }

            var hasDuplicateParameterName = GenericParametersAccessor
                .Any(gp => gp.Name == name);

            if (hasDuplicateParameterName)
            {
                throw new InvalidOperationException(
                    $"Method '{DeclaringTypeExpression.Name}.{Name}': " +
                    $"duplicate generic parameter name '{name}' specified.");
            }
        }

        private void ThrowIfTypeGenericParameterNameClash(string genericParameterName)
        {
            if (!DeclaringTypeExpression.IsGeneric)
            {
                return;
            }

            var hasClashingGenericParameter = DeclaringTypeExpression
                .GenericParameters
                .Any(gp => gp.Name == genericParameterName);

            if (hasClashingGenericParameter)
            {
                throw new InvalidOperationException(
                    $"Generic parameter '{genericParameterName}' has the same name as a " +
                    $"generic parameter in declaring type '{DeclaringTypeExpression.Name}'.");
            }
        }

        #endregion

        #region IMethodExpressionConfigurator Members

        void IMethodExpressionConfigurator.AddParameters(params ParameterExpression[] parameters)
            => AddParameters(parameters);

        #endregion

        #region IConcreteTypeMethodExpressionConfigurator Members

        void IConcreteTypeMethodExpressionConfigurator.SetStatic()
        {
            this.ValidateSetStatic();
            SetStatic();
        }

        void IConcreteTypeMethodExpressionConfigurator.SetBody(Expression body, Type returnType)
            => SetBody(body, returnType);

        #endregion

        internal string GetSignature() => GetSignature(includeTypeName: true);

        /// <inheritdoc />
        protected override string GetSignature(bool includeTypeName)
            => this.GetMethodSignature(includeTypeName);

        #region IMember Members

        IType IMember.DeclaringType => DeclaringTypeExpression;

        IType IMember.Type => GetReturnType();

        #endregion

        #region IComplexMember Members

        bool IComplexMember.IsOverride => IsOverride;

        #endregion

        #region IMethodBase Members

        bool IMethodBase.IsExtensionMethod => false;

        #endregion

        #region IMethod Members

        /// <inheritdoc />
        protected override ReadOnlyCollection<IGenericParameter> GetGenericArguments()
        {
            return _readOnlyIGenericParameters ??= IsGeneric
                ? _genericParameters.ProjectToArray(arg => (IGenericParameter)arg).ToReadOnlyCollection()
                : Enumerable<IGenericParameter>.EmptyReadOnlyCollection;
        }

        /// <inheritdoc />
        protected override IType GetReturnType() => _returnType ??= BclTypeWrapper.For(ReturnType);

        #endregion

        #region Validation

        /// <inheritdoc />
        protected override string MethodTypeName => "method";

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new MethodTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => new ReturnDefaultMethodTranslation(this, context);

        #endregion

        internal bool Equals(MethodExpression method)
        {
            if (Name != method.Name)
            {
                return false;
            }

            if (IsGeneric != method.IsGeneric)
            {
                return false;
            }

            if (IsGeneric && !_genericParameters.SequenceEqual(method._genericParameters))
            {
                return false;
            }

            return HasSameParameterTypesAs(method);
        }

        bool IEquatable<MethodExpression>.Equals(MethodExpression other) => Equals(other);
    }
}