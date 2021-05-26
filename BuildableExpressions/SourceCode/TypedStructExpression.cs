namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedStructExpression : StructExpression, ITypedTypeExpression
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

        bool IType.IsPrimitive => _structType.IsPrimitive;

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new TypeNameTranslation(_structType, context.Settings);
    }
}