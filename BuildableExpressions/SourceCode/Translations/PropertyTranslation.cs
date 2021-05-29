namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System.Linq;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    internal class PropertyTranslation : PropertyDefinitionTranslation
    {
        private readonly IPotentialEmptyTranslatable _attributesTranslation;
        private readonly bool _isAutoProperty;

        public PropertyTranslation(
            PropertyExpression propertyExpression,
            ITranslationContext context)
            : base(
                propertyExpression,
                propertyExpression.GetAccessors().ToList(),
                includeDeclaringType: false,
                (p, acc, _) => PropertyAccessorTranslation.For(propertyExpression, acc, p, context),
                context.Settings)
        {
            _attributesTranslation = AttributeSetTranslation.For(propertyExpression, context);
            _isAutoProperty = propertyExpression.IsAutoProperty;
        }

        protected override void WritePropertyStartTo(TranslationWriter writer) 
            => _attributesTranslation.WriteWithNewLineIfNotEmptyTo(writer);

        protected override void WriteAccessorsStartTo(TranslationWriter writer)
        {
            if (_isAutoProperty)
            {
                base.WriteAccessorsStartTo(writer);
                return;
            }

            writer.WriteOpeningBraceToTranslation();
        }

        protected override void WriteAccessorsEndTo(TranslationWriter writer)
        {
            if (_isAutoProperty)
            {
                base.WriteAccessorsEndTo(writer);
                return;
            }

            writer.WriteClosingBraceToTranslation();
        }

        private class PropertyAccessorTranslation : PropertyAccessorDefinitionTranslation
        {
            private readonly ITranslation _accessorTranslation;

            private PropertyAccessorTranslation(
                PropertyDefinitionTranslation parent,
                PropertyExpression propertyExpression,
                IMember accessor,
                ITranslationContext context)
                : base(parent, accessor, context.Settings)
            {
                var isGetter = IsGetter(accessor);

                var accessorExpression = isGetter
                    ? propertyExpression.GetterExpression
                    : propertyExpression.SetterExpression;

                var accessorCodeBlock = context
                    .GetCodeBlockTranslationFor(accessorExpression)
                    .WithBraces()
                    .WithTermination();

                if (isGetter)
                {
                    accessorCodeBlock.WithReturnKeyword();
                }

                TranslationSize = base.TranslationSize + accessorCodeBlock.TranslationSize;
                FormattingSize = base.FormattingSize + accessorCodeBlock.FormattingSize;
                _accessorTranslation = accessorCodeBlock;
            }

            #region Factory Method

            public static ITranslatable For(
                PropertyExpression propertyExpression,
                IMember accessor,
                PropertyDefinitionTranslation parent,
                ITranslationContext context)
            {
                if (propertyExpression.IsAutoProperty)
                {
                    return new PropertyAccessorDefinitionTranslation(
                        parent,
                        accessor,
                        context.Settings);
                }

                return new PropertyAccessorTranslation(
                    parent,
                    propertyExpression,
                    accessor,
                    context);
            }

            #endregion

            public override int TranslationSize { get; }

            public override int FormattingSize { get; }

            public override void WriteTo(TranslationWriter writer)
            {
                WriteAccessorTo(writer);
                _accessorTranslation.WriteTo(writer);
            }
        }
    }
}