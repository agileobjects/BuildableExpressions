namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedStructExpression : StructExpression, IType
    {
        private readonly Type _structType;

        public TypedStructExpression(Type structType)
            : base(structType)
        {
            _structType = structType;
        }

        #region IType Members

        string IType.FullName => _structType.FullName;

        string IType.Name => _structType.Name;

        bool IType.IsNested => _structType.IsNested;

        #endregion

        protected override TypeExpression CreateInstance() 
            => new TypedStructExpression(_structType);
    }
}