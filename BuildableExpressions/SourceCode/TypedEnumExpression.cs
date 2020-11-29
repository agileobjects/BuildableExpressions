namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedEnumExpression : EnumExpression, IType
    {
        private readonly Type _enumType;

        public TypedEnumExpression(Type enumType)
            : base(enumType)
        {
            _enumType = enumType;
        }

        #region IType Members

        string IType.FullName => _enumType.FullName;

        #endregion

        protected override TypeExpression CreateInstance()
            => new TypedEnumExpression(Type);

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new TypeNameTranslation(_enumType, context.Settings);
    }
}