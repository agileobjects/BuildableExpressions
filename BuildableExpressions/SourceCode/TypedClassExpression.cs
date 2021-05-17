namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedClassExpression : ClassExpression, ITypedTypeExpression
    {
        private readonly Type _classType;
        private IType _elementType;

        public TypedClassExpression(Type classType)
            : base(classType)
        {
            _classType = classType;

            var baseType = classType.GetBaseType();

            if (baseType != typeof(object))
            {
                BaseTypeExpression = TypeExpressionFactory.CreateClass(baseType);
            }

            var isAbstract = classType.IsAbstract();
            var isSealed = classType.IsSealed();
            IsStatic = isAbstract && isSealed;

            if (!IsStatic)
            {
                IsAbstract = isAbstract;
                IsSealed = isSealed;
            }
        }

        #region IType Members

        string IType.FullName => _classType.FullName;

        string IType.Name => _classType.Name;

        bool IType.IsArray => _classType.IsArray;

        IType IType.ElementType
            => _elementType ??= ClrTypeWrapper.For(_classType.GetEnumerableElementType());

        bool IType.IsAnonymous => _classType.IsAnonymous();

        bool IType.IsEnumerable => _classType.IsEnumerable();

        bool IType.IsDictionary => _classType.IsDictionary();

        bool IType.IsNested => _classType.IsNested;

        #endregion

        #region IClosableTypeExpression Members

        protected override ClassExpression CreateInstance()
            => new TypedClassExpression(_classType);

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new TypeNameTranslation(_classType, context.Settings);
    }
}