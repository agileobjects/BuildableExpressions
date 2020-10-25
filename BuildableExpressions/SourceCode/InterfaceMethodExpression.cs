namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Analysis;
    using Extensions;

    internal class InterfaceMethodExpression : MethodExpression
    {
        public InterfaceMethodExpression(
            InterfaceExpression declaringInterfaceExpression,
            string name,
            Type returnType,
            Action<MethodExpression> configuration)
            : base(declaringInterfaceExpression, name, configuration)
        {
            ReturnType = returnType;
            Analysis = MethodExpressionAnalysis.For(this);
            Validate();
        }

        #region Validation

        private void Validate()
            => ThrowIfDuplicateMethodSignature();

        #endregion

        /// <inheritdoc />
        public override Type ReturnType { get; }

        internal override bool HasGeneratedName => false;

        internal override bool HasBody => false;

        public override ReadOnlyCollection<ParameterExpression> Parameters
            => ParametersAccessor?.ToReadOnlyCollection();

        protected override IEnumerable<Expression> GetAnalysisExpressions()
            => (IList<Expression>)ParametersAccessor ?? Enumerable<Expression>.EmptyArray;
    }
}