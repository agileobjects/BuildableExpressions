namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Analysis;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;

    internal class StandardMethodExpression : MethodExpression
    {
        private bool? _isOverride;
        private List<BlockMethodExpression> _blockMethods;
        private Type[] _parameterTypes;

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
            if (Equals(otherMethod, this))
            {
                return false;
            }

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