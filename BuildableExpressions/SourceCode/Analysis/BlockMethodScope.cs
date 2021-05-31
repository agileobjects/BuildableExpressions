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

        public BlockMethodExpression BlockMethod { get; private set; }

        public override MethodExpressionBase RootMethodExpression
            => Parent.RootMethodExpression;

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

        public override void FinaliseBody(Expression finalisedBody)
        {
            base.FinaliseBody(finalisedBody);

            var owningTypeExpression = RootMethodExpression.DeclaringTypeExpression;

            BlockMethod = new BlockMethodExpression(owningTypeExpression, m =>
            {
                m.SetVisibility(MemberVisibility.Private);

                if (RootMethodExpression.IsStatic)
                {
                    m.SetStatic();
                }

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

            AddBlockMethodTo(owningTypeExpression);

            if (_childBlockScopes == null)
            {
                return;
            }

            foreach (var childBlockScope in _childBlockScopes)
            {
                childBlockScope.AddBlockMethodTo(owningTypeExpression);
            }
        }

        private void AddBlockMethodTo(TypeExpression owningTypeExpression)
            => owningTypeExpression.AddMethod(BlockMethod);

        protected override void UnscopedVariablesAccessed(
            IEnumerable<ParameterExpression> unscopedVariables)
        {
            _parameters = new List<ParameterExpression>(unscopedVariables);
        }
    }
}