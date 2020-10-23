namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class BlockMethodScope : MethodScopeBase
    {
        private readonly bool _isNestedBlock;
        private List<ParameterExpression> _parameters;
        private List<BlockMethodScope> _childBlockScopes;

        public BlockMethodScope(MethodScopeBase parent)
            : base(parent)
        {
            if (parent is BlockMethodScope parentBlockScope)
            {
                _isNestedBlock = true;
                parentBlockScope.AddChildBlockScope(this);
            }
        }

        public MethodExpression BlockMethod { get; private set; }

        public override MethodExpression GetOwningMethod()
            => Parent.GetOwningMethod();

        private void AddChildBlockScope(BlockMethodScope childBlockScope)
        {
            _childBlockScopes ??= new List<BlockMethodScope>();
            _childBlockScopes.Add(childBlockScope);
        }

        protected override void UnscopedVariableAccessed(ParameterExpression variable)
        {
            base.UnscopedVariableAccessed(variable);
            Parent.VariableAccessed(variable);
        }

        public override void Finalise(Expression finalisedBody)
        {
            base.Finalise(finalisedBody);

            var owningMethod = GetOwningMethod();

            BlockMethod = new MethodExpression(owningMethod.DeclaringTypeExpression, m =>
            {
                m.SetVisibility(MemberVisibility.Private);

                if (_parameters != null)
                {
                    m.AddParameters(_parameters);
                }

                m.SetBody(finalisedBody);
            });

            if (_isNestedBlock)
            {
                return;
            }

            Finalise(owningMethod);

            if (_childBlockScopes == null)
            {
                return;
            }

            foreach (var childBlockScope in _childBlockScopes)
            {
                childBlockScope.Finalise(owningMethod);
            }
        }

        private void Finalise(MethodExpression owningMethod)
            => owningMethod.BlockMethods.Add(BlockMethod);

        protected override void UnscopedVariablesAccessed(
            IEnumerable<ParameterExpression> unscopedVariables)
        {
            _parameters = new List<ParameterExpression>(unscopedVariables);
        }
    }
}