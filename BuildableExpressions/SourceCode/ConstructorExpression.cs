namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    /// <summary>
    /// Represents a class or struct constructor in a piece of source code.
    /// </summary>
    public class ConstructorExpression :
        MethodExpression,
        IConstructorExpressionConfigurator,
        IConstructor
    {
        private string _name;

        internal ConstructorExpression(
            TypeExpression declaringTypeExpression,
            Action<ConstructorExpression> configuration)
            : base(declaringTypeExpression, string.Empty)
        {
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(MemberVisibility.Public);
            }
        }

        /// <inheritdoc />
        public override string Name => _name ??= GenerateName();

        private string GenerateName()
        {
            if (ParametersAccessor == null)
            {
                return "Ctor()";
            }

            var parameterTypeNames = ParametersAccessor
                .Project(p => p.Type.GetFriendlyName());

            return $"Ctor({string.Join(", ", parameterTypeNames)})";
        }

        internal override bool HasGeneratedName => false;

        internal override bool HasBody => Body != null;

        /// <inheritdoc cref="IComplexMember.IsOverride" />
        public override bool IsOverride => false;

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new ConstructorTranslation(this, context);
    }
}