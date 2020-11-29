namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System.Collections.ObjectModel;
    using System.Reflection;
    using ReadableExpressions.Translations.Reflection;

    internal class GenericArgumentExpression : GenericParameterExpression, IType
    {
        private readonly IType _closedType;

        public GenericArgumentExpression(
            OpenGenericParameterExpression genericParameter,
            TypeExpression closedTypeExpression)
            : base(genericParameter.SourceCode, closedTypeExpression.Name)
        {
            GenericParameter = genericParameter;
            ClosedTypeExpression = closedTypeExpression;
            _closedType = closedTypeExpression;
        }

        public OpenGenericParameterExpression GenericParameter { get; }

        public TypeExpression ClosedTypeExpression { get; }

        public override bool HasConstraints => GenericParameter.HasConstraints;

        public override bool HasClassConstraint => GenericParameter.HasClassConstraint;

        public override bool HasStructConstraint => GenericParameter.HasStructConstraint;

        public override bool HasNewableConstraint => GenericParameter.HasNewableConstraint;

        public override ReadOnlyCollection<IType> TypeConstraints
            => GenericParameter.TypeConstraints;

        #region IType Members

        string IType.Namespace => _closedType.Namespace;

        IType IType.BaseType => _closedType.BaseType;

        ReadOnlyCollection<IType> IType.AllInterfaces => _closedType.AllInterfaces;

        string IType.FullName => _closedType.FullName;

        string IType.Name => _closedType.Name;

        bool IType.IsInterface => _closedType.IsInterface;

        bool IType.IsClass => _closedType.IsClass;

        bool IType.IsPrimitive => _closedType.IsPrimitive;

        bool IType.IsAnonymous => _closedType.IsAnonymous;

        bool IType.IsEnumerable => _closedType.IsEnumerable;

        bool IType.IsDictionary => _closedType.IsDictionary;

        bool IType.IsGeneric => _closedType.IsGeneric;

        IType IType.GenericDefinition => GenericParameter;

        int IType.GenericParameterCount => _closedType.GenericParameterCount;

        ReadOnlyCollection<IType> IType.GenericTypeArguments => _closedType.GenericTypeArguments;

        GenericParameterAttributes IType.Constraints => _closedType.Constraints;

        ReadOnlyCollection<IType> IType.ConstraintTypes => _closedType.ConstraintTypes;

        bool IType.IsNested => _closedType.IsNested;

        IType IType.DeclaringType => _closedType.DeclaringType;

        bool IType.IsArray => _closedType.IsArray;

        IType IType.ElementType => _closedType.ElementType;

        bool IType.IsObjectType => _closedType.IsObjectType;

        bool IType.IsNullable => _closedType.IsNullable;

        IType IType.NonNullableUnderlyingType => _closedType.NonNullableUnderlyingType;

        bool IType.IsByRef => _closedType.IsByRef;

        #endregion

        protected override TypeExpression CreateInstance()
            => new GenericArgumentExpression(GenericParameter, ClosedTypeExpression);
    }
}