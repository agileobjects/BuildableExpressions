namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions.Extensions;

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

        public override TypeExpression GetDeclaringType() => Parent.GetDeclaringType();

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

            var declaringType = GetDeclaringType();

            BlockMethod = new MethodExpression(declaringType, m =>
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

            Finalise(declaringType);

            if (_childBlockScopes == null)
            {
                return;
            }

            foreach (var childBlockScope in _childBlockScopes)
            {
                childBlockScope.Finalise(declaringType);
            }
        }

        private void Finalise(TypeExpression declaringType)
        {
            BlockMethod.Name = GetName();
            declaringType.Register(BlockMethod);
        }

        private string GetName()
        {
            var baseName = GetBaseName();

            var latestMatchingMethodSuffix =
                GetLatestMatchingMethodSuffix(baseName);

            if (latestMatchingMethodSuffix == 0)
            {
                return baseName;
            }

            return baseName + (latestMatchingMethodSuffix + 1);
        }

        private int GetLatestMatchingMethodSuffix(string baseName)
        {
            var parameterTypes =
                _parameters?.Project(p => p.Type).ToList() ??
                Enumerable<Type>.EmptyList;

            return BlockMethod
                .DeclaringTypeExpression
                .MethodExpressions
                .Filter(m => m.Name != null)
                .Select(m =>
                {
                    if (m.Name == baseName)
                    {
                        if (m.IsBlockMethod)
                        {
                            m.Name += "1";
                        }

                        return new { Suffix = 1 };
                    }

                    if (!m.Name.StartsWith(baseName, StringComparison.Ordinal))
                    {
                        return null;
                    }

                    var methodNameSuffix = m.Name.Substring(baseName.Length);

                    if (!int.TryParse(methodNameSuffix, out var suffix))
                    {
                        return null;
                    }

                    if (!m.Parameters.Project(p => p.Type).SequenceEqual(parameterTypes))
                    {
                        return null;
                    }

                    return new { Suffix = suffix };
                })
                .Filter(_ => _ != null)
                .Select(_ => _.Suffix)
                .OrderByDescending(suffix => suffix)
                .FirstOrDefault();
        }

        private string GetBaseName()
        {
            var body = BlockMethod.Definition.Body;

            return body.HasReturnType()
                ? "Get" + body.Type.GetVariableNameInPascalCase()
                : "DoAction";
        }

        protected override void UnscopedVariablesAccessed(
            IEnumerable<ParameterExpression> unscopedVariables)
        {
            _parameters = new List<ParameterExpression>(unscopedVariables);
        }
    }
}