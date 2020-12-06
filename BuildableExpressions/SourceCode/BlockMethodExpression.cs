namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;

    internal class BlockMethodExpression : MethodExpression
    {
        public BlockMethodExpression(
            TypeExpression declaringTypeExpression,
            Action<IConcreteTypeMethodExpressionConfigurator> configuration)
            : base(declaringTypeExpression, name: null)
        {
            configuration.Invoke(this);
            CallExpression = new BlockMethodCallExpression(this);
        }

        internal override bool HasGeneratedName => true;

        internal override bool HasBody => true;

        public override bool IsOverride => false;

        public Expression CallExpression { get; }

        public void Finalise() => SetName(GenerateName());

        #region Name Generation

        private string GenerateName()
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

        private string GetBaseName()
        {
            return ReturnType != typeof(void)
                ? "Get" + ReturnType.GetVariableNameInPascalCase()
                : "DoAction";
        }

        private int GetLatestMatchingMethodSuffix(string baseName)
        {
            var parameterTypes =
                ParametersAccessor?.ProjectToArray(p => p.Type) ??
                Type.EmptyTypes;

            return DeclaringTypeExpression
                .MethodExpressions
                .Filter(m => m.Name != null)
                .Select(m =>
                {
                    if (m.Name == baseName)
                    {
                        if (m.HasGeneratedName)
                        {
                            m.SetName(m.Name + "1");
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

                    if (m.ParametersAccessor?.Project(p => p.Type).SequenceEqual(parameterTypes) != true)
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

        #endregion

        #region Validation

        /// <inheritdoc />
        protected override IEnumerable<MethodExpression> SiblingMethodExpressions
            => null;

        /// <inheritdoc />
        protected override string MethodTypeName => "method";

        #endregion
    }
}