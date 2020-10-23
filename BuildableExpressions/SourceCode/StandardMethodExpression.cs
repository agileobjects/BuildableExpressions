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
            ThrowIfDuplicateGenericArgumentNames();
            ThrowIfDuplicateMethodName();
        }

        private void ThrowIfDuplicateGenericArgumentNames()
        {
            if (!IsGeneric || !(GenericArgumentsAccessor?.Count > 1))
            {
                return;
            }

            var duplicateParameterName = GenericArgumentsAccessor
                .GroupBy(arg => arg.Name)
                .FirstOrDefault(nameGroup => nameGroup.Count() > 1)?
                .Key;

            if (duplicateParameterName != null)
            {
                throw new InvalidOperationException(
                    $"Method '{DeclaringTypeExpression.Name}.{Name}': " +
                    $"duplicate generic parameter name '{duplicateParameterName}' specified.");
            }
        }

        private void ThrowIfDuplicateMethodName()
        {
            var duplicateMethod = DeclaringTypeExpression
                .MethodExpressions
                .FirstOrDefault(m => m != this && m.Name == Name && HasSameParameterTypes(m));

            if (duplicateMethod != null)
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