namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedInterfaceExpression : InterfaceExpression, IType
    {
        private readonly Type _interfaceType;

        public TypedInterfaceExpression(Type interfaceType)
            : base(interfaceType)
        {
            _interfaceType = interfaceType;
        }

        #region IType Members

        string IType.FullName => _interfaceType.FullName;

        string IType.Name => _interfaceType.Name;

        bool IType.IsNested => _interfaceType.IsNested;

        #endregion

        protected override TypeExpression CreateInstance()
            => new TypedInterfaceExpression(_interfaceType);
    }
}