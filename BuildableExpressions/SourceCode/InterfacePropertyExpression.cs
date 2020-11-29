namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations.Reflection;
    using static MemberVisibility;

    internal class InterfacePropertyExpression :
        PropertyExpression,
        IInterfacePropertyExpressionConfigurator
    {
        public InterfacePropertyExpression(
            InterfaceExpression declaringInterfaceExpression,
            string name,
            IType type,
            Action<InterfacePropertyExpression> configuration)
            : base(declaringInterfaceExpression, name, type)
        {
            configuration.Invoke(this);
            SetVisibility(Public);
            SetAbstract();
        }

        public override bool IsOverride => false;

        public override bool IsAutoProperty => true;

        #region IInterfacePropertyExpressionConfigurator Members

        void IInterfacePropertyExpressionConfigurator.SetGetter()
            => SetGetter(g => { });

        void IInterfacePropertyExpressionConfigurator.SetSetter()
            => SetSetter(s => { });

        #endregion
    }
}