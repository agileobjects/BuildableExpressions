namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedClassExpression : ClassExpression, IType
    {
        private readonly Type _classType;

        public TypedClassExpression(Type classType)
            : base(classType)
        {
            _classType = classType;
            BaseTypeExpression = GetBaseTypeExpression(classType.GetBaseType());

            var isAbstract = classType.IsAbstract();
            var isSealed = classType.IsSealed();
            IsStatic = isAbstract && isSealed;

            if (!IsStatic)
            {
                IsAbstract = isAbstract;
                IsSealed = isSealed;
            }
        }

        #region Setup

        private static ClassExpression GetBaseTypeExpression(Type baseType)
        {
            return baseType != typeof(object)
                ? new TypedClassExpression(baseType) : null;
        }

        #endregion

        #region IType Members

        string IType.FullName => _classType.FullName;

        string IType.Name => _classType.Name;

        bool IType.IsNested => _classType.IsNested;

        #endregion

        protected override TypeExpression CreateInstance()
            => new TypedClassExpression(_classType);
    }
}