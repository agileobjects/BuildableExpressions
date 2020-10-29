namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Collections.ObjectModel;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class ClosedGenericTypeArgumentExpression : GenericParameterExpression
    {
        private readonly IGenericArgument _parameter;

        public ClosedGenericTypeArgumentExpression(
            OpenGenericArgumentExpression parameterExpression,
            Type type)
            : base(type.GetFriendlyName())
        {
            _parameter = parameterExpression;
            Type = type;
            ParameterExpression = parameterExpression;
        }

        public override Type Type { get; }

        public OpenGenericArgumentExpression ParameterExpression { get; }

        public override bool IsClosed => true;

        #region IGenericArgument Members

        protected override bool HasConstraints => _parameter.HasConstraints;

        protected override bool HasClassConstraint => _parameter.HasClassConstraint;

        protected override bool HasStructConstraint => _parameter.HasStructConstraint;

        protected override bool HasNewableConstraint => _parameter.HasNewableConstraint;

        protected override ReadOnlyCollection<Type> TypeConstraints
            => _parameter.TypeConstraints;

        #endregion
    }
}