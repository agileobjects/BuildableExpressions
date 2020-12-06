namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
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
        IClassConstructorExpressionConfigurator,
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

            Validate();
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1005) indicating the type of this
        /// <see cref="ConstructorExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Constructor;

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

        internal override bool HasBody => !IsAbstract;

        /// <inheritdoc cref="IComplexMember.IsOverride" />
        public override bool IsOverride => false;

        #region Validation

        /// <inheritdoc />
        protected override IEnumerable<MethodExpression> SiblingMethodExpressions
            => DeclaringTypeExpression.ConstructorExpressions;

        /// <inheritdoc />
        protected override string MethodTypeName => "constructor";

        #endregion

        #region IClassConstructorExpressionConfigurator Members

        void IConstructorExpressionConfigurator.AddParameters(
            params ParameterExpression[] parameters)
        {
            AddParameters(new List<ParameterExpression>(parameters));
        }

        void IConstructorExpressionConfigurator.SetBody(Expression body)
            => SetBody(body, typeof(void));

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new ConstructorTranslation(this, context);

        #endregion

        /// <inheritdoc />
        public override string ToString() => Name;
    }
}