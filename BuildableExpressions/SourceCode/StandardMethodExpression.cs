namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Analysis;
    using Api;
    using BuildableExpressions.Extensions;

    internal class StandardMethodExpression : MethodExpression
    {
        private List<BlockMethodExpression> _blockMethods;

        public StandardMethodExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Action<IMethodExpressionConfigurator> configuration)
            : base(
                declaringTypeExpression,
                name.ThrowIfInvalidName<ArgumentException>("Method"),
                configuration)
        {
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
            if (Body == null)
            {
                throw new InvalidOperationException(
                    $"Method '{this.GetSignature()}': no method body defined. " +
                    "To add an empty method, use SetBody(Expression.Default(typeof(void)))");
            }
        }

        private void ThrowIfDuplicateMethodSignature()
        {
            var hasDuplicateMethod = DeclaringTypeExpression
                .MethodExpressions
                .Any(m => m.Name == Name && HasSameParameterTypes(m));

            if (hasDuplicateMethod)
            {
                throw new InvalidOperationException(
                    $"Type {DeclaringTypeExpression.Name} has duplicate " +
                    $"method signature '{this.GetSignature(includeTypeName: false)}'");
            }
        }

        private bool HasSameParameterTypes(MethodExpression otherMethod)
        {
            if (ParametersAccessor == null)
            {
                return otherMethod.ParametersAccessor == null;
            }

            if (otherMethod.ParametersAccessor == null)
            {
                return false;
            }

            var parameterTypes =
                ParametersAccessor.ProjectToArray(p => p.Type);

            return otherMethod.ParametersAccessor
                .Project(p => p.Type)
                .SequenceEqual(parameterTypes);
        }

        #endregion

        internal override bool HasGeneratedName => false;

        public bool HasBlockMethods => _blockMethods != null;

        public IList<BlockMethodExpression> BlockMethods
            => _blockMethods ??= new List<BlockMethodExpression>();

        public BlockMethodExpression CreateBlockMethod(
            Action<IMethodExpressionConfigurator> configuration)
        {
            return new BlockMethodExpression(DeclaringTypeExpression, configuration);
        }
    }
}