namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Reflection;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedAttributeExpression : AttributeExpression, ITypedTypeExpression
    {
        private AttributeTargets? _targets;
        private readonly Type _attributeType;

        public TypedAttributeExpression(Type attributeType)
            : base(attributeType)
        {
            _attributeType = attributeType;

            var baseType = attributeType.GetBaseType();

            if (baseType != typeof(object))
            {
                BaseTypeExpression = TypeExpressionFactory.CreateAttribute(baseType);
            }

            IsAbstract = attributeType.IsAbstract();
            IsSealed = attributeType.IsSealed();
        }

        public override AttributeTargets ValidOn => _targets ??= GetAttributeTargets();

        private AttributeTargets GetAttributeTargets()
        {
            return _attributeType
                .GetCustomAttribute<AttributeUsageAttribute>()?
                .ValidOn ?? AttributeTargets.All;
        }

        #region IType Members

        string IType.FullName => _attributeType.FullName;

        string IType.Name => _attributeType.Name;

        bool IType.IsNested => _attributeType.IsNested;

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new TypeNameTranslation(_attributeType, context.Settings);
    }
}