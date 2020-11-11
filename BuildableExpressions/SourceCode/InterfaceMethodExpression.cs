namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Analysis;
    using ReadableExpressions.Translations.Reflection;

    internal class InterfaceMethodExpression :
        MethodExpression,
        IPotentialInterfaceMember
    {
        public InterfaceMethodExpression(
            InterfaceExpression declaringInterfaceExpression,
            string name,
            Type returnType,
            Action<MethodExpression> configuration)
            : base(declaringInterfaceExpression, name)
        {
            configuration.Invoke(this);

            ReturnType = returnType;
            SetAbstract();
            Analysis = MethodExpressionAnalysis.For(this);
            Validate();
        }

        #region Validation

        private void Validate()
            => ThrowIfDuplicateMethodSignature();

        #endregion

        public override Type ReturnType { get; }

        internal override bool HasGeneratedName => false;

        internal override bool HasBody => false;

        public override bool IsOverride => false;

        bool IPotentialInterfaceMember.IsInterfaceMember => true;
    }
}