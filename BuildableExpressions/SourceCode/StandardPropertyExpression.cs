namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;
    using Api;
    using Extensions;
    using ReadableExpressions;
    using static MemberVisibility;

    internal class StandardPropertyExpression :
        PropertyExpression,
        IClassPropertyExpressionConfigurator,
        IConcreteTypeExpression
    {
        private bool? _isOverride;

        public StandardPropertyExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Type type,
            Action<StandardPropertyExpression> configuration)
            : base(declaringTypeExpression, name, type)
        {
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(Public);
            }

            if (GetterExpression == null && SetterExpression == null)
            {
                this.PublicGetSet();
            }

            if (IsAbstract)
            {
                return;
            }

            if (GetterExpression?.HasBody == false && SetterExpression == null)
            {
                SetSetter(s => s.SetVisibility(Private));
            }

            if (SetterExpression?.HasBody == false && GetterExpression == null)
            {
                SetGetter(s => s.SetVisibility(Private));
            }
        }

        public override bool IsOverride
            => _isOverride ??= DetermineIfOverride();

        private bool DetermineIfOverride()
        {
            return DeclaringTypeExpression
                .GetAllVirtualMembersOfType<StandardPropertyExpression>()
                .Any(p => p != this && p.Type == Type && p.Name == Name);
        }

        public override bool IsAutoProperty
            => GetterExpression?.HasBody != true && SetterExpression?.HasBody != true;

        #region IMemberExpressionConfigurator Members

        void IMemberExpressionConfigurator.SetSummary(CommentExpression summary)
            => SetSummary(summary);

        void IMemberExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        #endregion

        #region IConcreteTypePropertyExpressionConfigurator Members

        void IConcreteTypePropertyExpressionConfigurator.SetStatic()
        {
            this.ValidateSetStatic();
            SetStatic();
        }

        #endregion

        #region IClassMemberExpressionConfigurator Members

        void IClassMemberExpressionConfigurator.SetAbstract()
        {
            this.ValidateSetAbstract();
            SetAbstract();
        }

        void IClassMemberExpressionConfigurator.SetVirtual()
        {
            this.ValidateSetVirtual();
            SetVirtual();
        }

        void IConcreteTypePropertyExpressionConfigurator.SetGetter(
            Action<IPropertyGetterConfigurator> configuration)
        {
            SetGetter(configuration);
        }

        void IConcreteTypePropertyExpressionConfigurator.SetSetter(
            Action<IPropertySetterConfigurator> configuration)
        {
            SetSetter(configuration);
        }

        #endregion
    }
}