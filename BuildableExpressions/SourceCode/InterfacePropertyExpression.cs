namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations.Reflection;
    using static MemberVisibility;

    internal class InterfacePropertyExpression :
        PropertyExpression,
        IInterfacePropertyExpressionConfigurator,
        IPotentialInterfaceMember
    {
        public InterfacePropertyExpression(
            InterfaceExpression declaringInterfaceExpression,
            string name,
            Type type,
            Action<InterfacePropertyExpression> configuration)
            : base(
                declaringInterfaceExpression,
                name,
                type,
                p => configuration.Invoke((InterfacePropertyExpression)p))
        {
            SetVisibility(Public);
            SetAbstract();
        }

        public override bool IsAutoProperty => true;

        #region IInterfacePropertyExpressionConfigurator Members

        void IInterfacePropertyExpressionConfigurator.SetGetter()
            => SetGetter(g => { });

        void IInterfacePropertyExpressionConfigurator.SetSetter()
            => SetSetter(s => { });

        #endregion

        bool IPotentialInterfaceMember.IsInterfaceMember => true;
    }
}