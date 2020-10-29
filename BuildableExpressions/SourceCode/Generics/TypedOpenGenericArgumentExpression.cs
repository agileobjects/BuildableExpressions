namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedOpenGenericArgumentExpression : OpenGenericArgumentExpression
    {
        private readonly IGenericArgument _genericArgument;

        public TypedOpenGenericArgumentExpression(Type parameterType)
            : base(parameterType.Name)
        {
            _genericArgument = GenericArgumentFactory.For(parameterType);
            Type = parameterType;
        }

        public override Type Type { get; }

        protected override bool HasConstraints => _genericArgument.HasConstraints;

        protected override bool HasClassConstraint => _genericArgument.HasClassConstraint;

        protected override bool HasStructConstraint => _genericArgument.HasStructConstraint;

        protected override bool HasNewableConstraint => _genericArgument.HasNewableConstraint;

        public override IEnumerable<Type> TypeConstraintsAccessor => TypeConstraints;

        protected override ReadOnlyCollection<Type> TypeConstraints
            => _genericArgument.TypeConstraints;
    }
}