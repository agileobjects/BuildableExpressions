namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Represents a class or struct constructor in a piece of source code.
    /// </summary>
    public class ConstructorExpression :
        MethodExpression,
        IConstructorExpressionConfigurator,
        IConstructor
    {
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

        internal bool HasChainedConstructorCall => ChainedConstructorCall != null;

        /// <summary>
        /// Gets the <see cref="ChainedConstructorCallExpression"/> representing the chained call
        /// to base or sibling <see cref="ConstructorExpression"/>, if one exists.
        /// </summary>
        public ChainedConstructorCallExpression ChainedConstructorCall { get; private set; }

        /// <inheritdoc />
        public override string Name => DeclaringTypeExpression.Name;

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

        void IConstructorExpressionConfigurator.SetConstructorCall(
            ConstructorExpression targetConstructorExpression,
            params Expression[] arguments)
        {
            ThrowIfInvalidTarget(targetConstructorExpression);
            ThrowIfInvalidArguments(targetConstructorExpression, arguments);

            ChainedConstructorCall = new ChainedConstructorCallExpression(
                this,
                targetConstructorExpression,
                arguments);
        }

        #region Validation

        private void ThrowIfInvalidTarget(ConstructorExpression targetConstructor)
        {
            var targetCtorDeclaringType = targetConstructor.DeclaringTypeExpression;

            if (targetCtorDeclaringType == DeclaringTypeExpression)
            {
                return;
            }

            var isInaccessibleCtor = false;

            if ((DeclaringTypeExpression is ClassExpression declaringClassExpression) &&
                (declaringClassExpression.BaseTypeExpression == targetCtorDeclaringType))
            {
                if (targetConstructor.Visibility != Private)
                {
                    return;
                }

                isInaccessibleCtor = true;
            }

            var visibility = isInaccessibleCtor ? "private " : null;

            throw new InvalidOperationException(
                $"Constructor {this} cannot call " +
                $"{visibility}constructor {targetConstructor}.");
        }

        private static void ThrowIfInvalidArguments(
            ConstructorExpression targetConstructor,
            IList<Expression> arguments)
        {
            var parameterCount = targetConstructor.ParametersAccessor?.Count ?? 0;

            if (parameterCount != arguments.Count)
            {
                throw new InvalidOperationException(
                    $"Constructor {targetConstructor} requires {parameterCount} " +
                    $"parameter(s). {arguments.Count} were supplied.");
            }

            if (parameterCount == 0)
            {
                return;
            }

            for (var i = 0; i < parameterCount; ++i)
            {
                // ReSharper disable once PossibleNullReferenceException
                var parameterType = targetConstructor.ParametersAccessor[i].Type;
                var argumentType = arguments[i].Type;

                if (argumentType.IsAssignableTo(parameterType))
                {
                    continue;
                }

                var argumentTypes = string.Join(", ", arguments
                    .ProjectToArray(arg => arg.Type.GetFriendlyName()));

                throw new InvalidOperationException(
                    $"Constructor {targetConstructor} cannot be called with " +
                    $"argument(s) of Type {argumentTypes}.");
            }
        }

        #endregion

        void IConstructorExpressionConfigurator.SetBody(Expression body)
            => SetBody(body, typeof(void));

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new ConstructorTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => new TransientConstructorTranslation(this, context);

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            var parameters = string.Join(
                ", ",
                ParametersAccessor?
                    .Project(p => p.Type.GetFriendlyName()) ??
                     Array.Empty<string>());

            return $"{DeclaringTypeExpression.GetFriendlyName()}({parameters})";
        }
    }
}