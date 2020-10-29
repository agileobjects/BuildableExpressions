namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using Analysis;
    using Api;
    using Extensions;

    internal class StandardMethodExpression : MethodExpression
    {
        private List<BlockMethodExpression> _blockMethods;

        public StandardMethodExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Action<MethodExpression> configuration)
            : base(
                declaringTypeExpression,
                name.ThrowIfInvalidName<ArgumentException>("Method"),
                configuration)
        {
            if (!Visibility.HasValue)
            {
                SetVisibility(MemberVisibility.Public);
            }

            Analysis = MethodExpressionAnalysis.For(this);
            Validate();
        }

        #region Validation

        internal void Validate()
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

        public bool HasBlockMethods => _blockMethods != null;

        public IList<BlockMethodExpression> BlockMethods
            => _blockMethods ??= new List<BlockMethodExpression>();

        public BlockMethodExpression CreateBlockMethod(
            Action<IConcreteTypeMethodExpressionConfigurator> configuration)
        {
            return new BlockMethodExpression(DeclaringTypeExpression, configuration);
        }
    }
}