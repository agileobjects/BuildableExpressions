namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Collections.Generic;

    internal abstract class OpenGenericArgumentExpression : GenericParameterExpression
    {
        internal OpenGenericArgumentExpression(string name)
            : base(name)
        {
        }

        public override bool IsClosed => false;

        public abstract IEnumerable<Type> TypeConstraintsAccessor { get; }

        public ClosedGenericTypeArgumentExpression Close(Type type)
            => new ClosedGenericTypeArgumentExpression(this, type);
    }
}