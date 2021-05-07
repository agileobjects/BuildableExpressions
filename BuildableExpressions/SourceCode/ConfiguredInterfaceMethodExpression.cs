namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using ReadableExpressions.Translations.Reflection;

    internal class ConfiguredInterfaceMethodExpression : MethodExpression
    {
        private readonly IType _returnType;

        public ConfiguredInterfaceMethodExpression(
            InterfaceExpression declaringInterfaceExpression,
            string name,
            IType returnType,
            Action<MethodExpression> configuration)
            : base(declaringInterfaceExpression, name)
        {
            _returnType = returnType;
            configuration.Invoke(this);

            SetAbstract();
            ThrowIfDuplicateSignature();
        }

        public override Type ReturnType => _returnType.AsType();

        internal override bool HasBody => false;

        public override bool IsOverride => false;

        protected override IType GetReturnType() => _returnType;

        #region Validation

        /// <inheritdoc />
        protected override IEnumerable<MethodExpressionBase> SiblingMethodExpressions
            => DeclaringTypeExpression.MethodExpressionsAccessor;

        #endregion

        internal override void ResetMemberInfo() 
            => SetMethodInfo(null);
    }
}