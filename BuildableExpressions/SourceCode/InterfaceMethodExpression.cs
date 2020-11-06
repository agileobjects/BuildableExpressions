namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Analysis;
    using Extensions;
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
            : base(declaringInterfaceExpression, name, configuration)
        {
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

        public override ReadOnlyCollection<ParameterExpression> Parameters
            => ParametersAccessor.ToReadOnlyCollection();

        bool IPotentialInterfaceMember.IsInterfaceMember => true;
    }
}