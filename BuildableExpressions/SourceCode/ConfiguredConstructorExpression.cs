namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using BuildableExpressions.Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using static MemberVisibility;

    internal class ConfiguredConstructorExpression :
        ConstructorExpression,
        IConstructorExpressionConfigurator
    {
        private ConstructorInfo _ctorInfo;

        public ConfiguredConstructorExpression(
            TypeExpression declaringTypeExpression,
            Action<ConfiguredConstructorExpression> configuration)
            : base(declaringTypeExpression)
        {
            configuration.Invoke(this);

            if (!Visibility.HasValue)
            {
                SetVisibility(Public);
            }

            Validate();
        }

        public override ConstructorInfo ConstructorInfo
            => _ctorInfo ??= CreateConstructorInfo();

        #region ConstructorInfo Creation

        private ConstructorInfo CreateConstructorInfo()
        {
            var parameterTypes =
                ParametersAccessor?.ProjectToArray(p => p.Type) ??
                Type.EmptyTypes;

            var declaringType = DeclaringType.AsType();

            var ctor = Visibility == Public
                ? IsStatic
                    ? null
                    : declaringType.GetPublicInstanceConstructor(parameterTypes)
                : IsStatic
                    ? null
                    : declaringType.GetNonPublicInstanceConstructor(parameterTypes);

            return ctor;
        }

        #endregion

        #region IClassConstructorExpressionConfigurator Members

        void IConstructorExpressionConfigurator.SetStatic()
        {
            SetStatic();
        }

        void IConstructorExpressionConfigurator.AddParameters(
            params ParameterExpression[] parameters)
        {
            AddParameters(parameters);
        }

        void IConstructorExpressionConfigurator.SetConstructorCall(
            ConstructorExpression targetConstructorExpression,
            params Expression[] arguments)
        {
            ThrowIfInvalidTarget(targetConstructorExpression);
            ThrowIfInvalidArguments(targetConstructorExpression, arguments);

            SetChainedConstructorCall(new ChainedConstructorCallExpression(
                this,
                targetConstructorExpression,
                arguments));
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

        void IConstructorExpressionConfigurator.SetBody(Expression body) => SetBody(body);

        #endregion

        #region Validation

        /// <inheritdoc />
        protected override IEnumerable<MethodExpressionBase> SiblingMethodExpressions
            => DeclaringTypeExpression.ConstructorExpressionsAccessor;

        #endregion

        internal override void ResetMemberInfo() => _ctorInfo = null;
    }
}