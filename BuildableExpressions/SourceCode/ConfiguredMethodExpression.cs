namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class ConfiguredMethodExpression :
        MethodExpression,
        IClassMethodExpressionConfigurator,
        IMethodBase
    {
        private bool? _isOverride;
        private bool _isExtensionMethod;
        private IType _returnType;

        public ConfiguredMethodExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Action<ConfiguredMethodExpression> configuration)
            : base(declaringTypeExpression, name.ThrowIfInvalidName("Method"))
        {
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(MemberVisibility.Public);
            }

            Validate();
        }

        public override bool IsOverride
            => _isOverride ??= DetermineIfOverride();

        private bool DetermineIfOverride()
        {
            return DeclaringTypeExpression
                .GetAllVirtualMembersOfType<ConfiguredMethodExpression>()
                .Any(m => m.IsOverriddenBy(this));
        }

        private bool IsOverriddenBy(MethodExpressionBase otherMethod)
        {
            if (otherMethod.ReturnType != ReturnType ||
                otherMethod.Name != Name)
            {
                return false;
            }

            if (otherMethod.ParametersAccessor == null)
            {
                return true;
            }

            return HasSameParameterTypesAs(otherMethod);
        }

        public override Type ReturnType
            => _returnType?.AsType() ?? base.ReturnType;

        #region IClassMemberExpressionConfigurator Members

        void IClassMemberExpressionConfigurator.SetVirtual()
        {
            this.ValidateSetVirtual();
            SetVirtual();
        }

        #endregion

        #region IClassMethodExpressionConfigurator Members

        void IClassMethodExpressionConfigurator.SetAbstract(IType returnType)
        {
            this.ValidateSetAbstract();

            _returnType = returnType;
            SetAbstract();
        }

        void IClassMethodExpressionConfigurator.SetExtensionMethod()
        {
            ValidateSetExtensionMethod();

            SetStatic();
            _isExtensionMethod = true;
        }

        private void ValidateSetExtensionMethod()
        {
            if (!((ClassExpression)DeclaringTypeExpression).IsStatic)
            {
                throw new InvalidOperationException(
                    $"Unable to set method '{GetSignature(includeTypeName: true)}' " +
                     "to an extension method as declaring class is non-static.");
            }

            this.ValidateSetStatic();
        }

        protected override IType GetReturnType()
            => _returnType ??= ClrTypeWrapper.For(ReturnType);

        #endregion

        #region IMethodBase Members

        bool IMethodBase.IsExtensionMethod => _isExtensionMethod;

        #endregion

        #region Validation

        /// <inheritdoc />
        protected override IEnumerable<MethodExpressionBase> SiblingMethodExpressions
            => DeclaringTypeExpression.MethodExpressionsAccessor;

        #endregion

        internal override void ResetMemberInfo()
            => SetMethodInfo(null);
    }
}