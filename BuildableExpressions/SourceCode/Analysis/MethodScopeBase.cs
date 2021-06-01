namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal abstract class MethodScopeBase
    {
        private readonly List<ParameterExpression> _inScopeVariables;
        private readonly List<ParameterExpression> _unscopedVariables;

        protected MethodScopeBase(MethodScopeBase parent)
        {
            Parent = parent;
            _inScopeVariables = new List<ParameterExpression>();
            _unscopedVariables = new List<ParameterExpression>();
        }

        public MethodScopeBase Parent { get; }

        public abstract MethodExpressionBase RootMethodExpression { get; }

        public void Add(ParameterExpression inScopeVariable)
            => _inScopeVariables.Add(inScopeVariable);

        public void Add(IEnumerable<ParameterExpression> inScopeVariables)
            => _inScopeVariables.AddRange(inScopeVariables);

        public void VariableAccessed(ParameterExpression variable)
        {
            if (_inScopeVariables.Contains(variable) ||
                _unscopedVariables.Contains(variable))
            {
                return;
            }

            UnscopedVariableAccessed(variable);
        }

        protected virtual void UnscopedVariableAccessed(ParameterExpression variable)
            => _unscopedVariables.Add(variable);

        public bool IsMethodParameter(ParameterExpression parameter)
            => _unscopedVariables.Contains(parameter);

        public virtual void FinaliseBody(Expression finalisedBody)
        {
            if (_unscopedVariables.Any())
            {
                UnscopedVariablesAccessed(_unscopedVariables);
            }
        }

        protected abstract void UnscopedVariablesAccessed(
            IEnumerable<ParameterExpression> unscopedVariables);
    }
}