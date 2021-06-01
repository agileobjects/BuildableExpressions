namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Collections.Generic;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class AttributedParameterSetDefinitionTranslation : ParameterSetDefinitionTranslation
    {
        private readonly Dictionary<int, AttributeSetTranslation> _parameterAttributeTranslations;

        private AttributedParameterSetDefinitionTranslation(
            MethodExpressionBase methodExpression,
            IDictionary<int, IList<AppliedAttribute>> parameterAttributes,
            ITranslationContext context)
            : base(
                methodExpression,
                ((IMethodBase)methodExpression).GetParameters(),
                context.Settings)
        {
            _parameterAttributeTranslations =
                new Dictionary<int, AttributeSetTranslation>(parameterAttributes.Count);

            foreach (var indexAndAttributes in parameterAttributes)
            {
                _parameterAttributeTranslations.Add(
                    indexAndAttributes.Key,
                    new AttributeSetTranslation(indexAndAttributes.Value, context));
            }
        }

        #region Factory Method

        public static ITranslatable For(
            MethodExpressionBase methodExpression,
            ITranslationContext context)
        {
            var parameters = methodExpression.ParametersAccessor;

            if (parameters == null)
            {
                return Empty;
            }

            var parameterAttributes = default(Dictionary<int, IList<AppliedAttribute>>);
            var parameterCount = parameters.Count;

            for (var i = 0; i < parameterCount; ++i)
            {
                var parameter = parameters[i];

                if (!methodExpression.TryGetAttributes(parameter, out var attributes))
                {
                    continue;
                }

                parameterAttributes ??= new Dictionary<int, IList<AppliedAttribute>>();
                parameterAttributes[i] = attributes;
            }

            if (parameterAttributes == null)
            {
                return For(methodExpression, context.Settings);
            }

            return new AttributedParameterSetDefinitionTranslation(
                methodExpression,
                parameterAttributes,
                context);
        }

        #endregion

        protected override void WriteParameterStartTo(TranslationWriter writer, int parameterIndex)
            => _parameterAttributeTranslations[parameterIndex].WriteSingleLineTo(writer);
    }
}