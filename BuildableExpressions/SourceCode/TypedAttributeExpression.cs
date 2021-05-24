namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class TypedAttributeExpression : AttributeExpression, ITypedTypeExpression
    {
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

        #region IType Members

        string IType.FullName => _attributeType.FullName;

        string IType.Name => _attributeType.Name;

        bool IType.IsNested => _attributeType.IsNested;

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new TypeNameTranslation(_attributeType, context.Settings);
    }
}