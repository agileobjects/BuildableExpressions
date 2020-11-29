namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Collections.ObjectModel;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedOpenGenericParameterExpression : OpenGenericParameterExpression, IType
    {
        private readonly IGenericParameter _genericParameter;
        private readonly Type _parameterType;

        public TypedOpenGenericParameterExpression(Type parameterType)
            : this(GenericParameterFactory.For(parameterType), parameterType)
        {
        }

        private TypedOpenGenericParameterExpression(
            IGenericParameter genericParameter,
            Type parameterType)
            : base(parameterType)
        {
            _genericParameter = genericParameter;
            _parameterType = parameterType;
        }

        public override bool HasConstraints => _genericParameter.HasConstraints;

        public override bool HasClassConstraint => _genericParameter.HasClassConstraint;

        public override bool HasStructConstraint => _genericParameter.HasStructConstraint;

        public override bool HasNewableConstraint => _genericParameter.HasNewableConstraint;

        public override ReadOnlyCollection<IType> TypeConstraints
            => _genericParameter.ConstraintTypes;

        #region IType Members

        string IType.FullName => _parameterType.FullName;

        string IType.Name => _parameterType.Name;

        bool IType.IsInterface => _parameterType.IsInterface();

        bool IType.IsClass => _parameterType.IsClass();

        bool IType.IsEnum => _parameterType.IsEnum();

        #endregion

        internal virtual OpenGenericParameterExpression Clone()
            => new TypedOpenGenericParameterExpression(_genericParameter, _parameterType);

        protected override TypeExpression CreateInstance() => Clone();
    }
}