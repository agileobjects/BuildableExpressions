namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class StandardMethodExpression :
        MethodExpression,
        IClassMethodExpressionConfigurator
    {
        private bool? _isOverride;
        private IType _returnType;
        private Type[] _parameterTypes;

        public StandardMethodExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Action<StandardMethodExpression> configuration)
            : base(
                declaringTypeExpression,
                name.ThrowIfInvalidName<ArgumentException>("Method"))
        {
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(MemberVisibility.Public);
            }

            Validate();
        }

        #region Validation

        private void Validate()
        {
            ThrowIfEmptyMethod();
            ThrowIfDuplicateMethodSignature();
        }

        private void ThrowIfEmptyMethod()
        {
            if (!IsAbstract && Body == null)
            {
                throw new InvalidOperationException(
                    $"Method '{this.GetSignature()}': no method body defined. " +
                    "To add an empty method, use SetBody(Expression.Default(typeof(void)))");
            }
        }

        #endregion

        internal override bool HasGeneratedName => false;

        internal override bool HasBody => !IsAbstract;

        public override bool IsOverride
            => _isOverride ??= DetermineIfOverride();

        private bool DetermineIfOverride()
        {
            return DeclaringTypeExpression
                .GetAllVirtualMembersOfType<StandardMethodExpression>()
                .Any(m => m.IsOverriddenBy(this));
        }

        private bool IsOverriddenBy(StandardMethodExpression otherMethod)
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

            return ParametersAccessor != null &&
                   ParameterTypes.SequenceEqual(otherMethod.ParameterTypes);
        }

        private IEnumerable<Type> ParameterTypes
            => _parameterTypes ??= ParametersAccessor.ProjectToArray(p => p.Type);

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

        protected override IType GetReturnType()
            => _returnType ??= BclTypeWrapper.For(ReturnType);

        #endregion
    }
}