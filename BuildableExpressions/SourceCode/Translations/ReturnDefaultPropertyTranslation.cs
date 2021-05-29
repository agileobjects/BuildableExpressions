namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using AgileObjects.ReadableExpressions.Translations;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class ReturnDefaultPropertyTranslation : ITranslation
    {
        private readonly IProperty _property;
        private readonly ITranslatable _typeNameTranslation;
        private readonly bool _writeModifiers;
        private readonly string _accessibility;
        private readonly string _modifiers;
        private readonly string _accessors;

        public ReturnDefaultPropertyTranslation(
            IProperty property,
            ITranslationContext context)
        {
            _property = property;
            _typeNameTranslation = context.GetTranslationFor(property.Type);

            var translationSize = _typeNameTranslation.TranslationSize + 1;

            _writeModifiers = !(property is InterfacePropertyExpression);

            if (_writeModifiers)
            {
                _accessibility = property.GetAccessibilityForTranslation();
                _modifiers = property.GetModifiersForTranslation();
                translationSize += _accessibility.Length + _modifiers.Length;
            }

            var getter = CreateGetter(property.Getter);
            var setter = CreateSetter(property.Setter);

            _accessors = "{" + getter + setter + "}";

            TranslationSize =
                translationSize +
                property.Name.Length +
               _accessors.Length;
        }

        #region Setup

        private string CreateGetter(IMember getter)
        {
            return CreateAccessor(getter, acc =>
                "{return default(" + acc.Type.GetFriendlyName() + ")}");
        }

        private string CreateSetter(IMember setter)
            => CreateAccessor(setter, _ => "{}");

        private string CreateAccessor(
            IMember accessor,
            Func<IMember, string> bodyFactory)
        {
            if (accessor == null)
            {
                return string.Empty;
            }

            var accessorName = accessor == _property.Getter ? "get" : "set";

            var accessorTranslation = accessorName + (_property.IsAutoProperty()
                ? ";" : bodyFactory.Invoke(accessor));

            if (_writeModifiers)
            {
                var accessorAccessibility = accessor.GetAccessibilityForTranslation();

                if (accessorAccessibility != _accessibility)
                {
                    accessorTranslation = accessorAccessibility + accessorTranslation;
                }
            }

            return accessorTranslation;
        }

        #endregion

        public ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Property;

        public Type Type => _property.Type.AsType();

        public int TranslationSize { get; }

        public int FormattingSize => 0;

        public int GetIndentSize() => 0;

        public int GetLineCount() => 1;

        public void WriteTo(TranslationWriter writer)
        {
            if (_writeModifiers)
            {
                writer.WriteToTranslation(_accessibility + _modifiers);
            }

            _typeNameTranslation.WriteTo(writer);
            writer.WriteSpaceToTranslation();
            writer.WriteToTranslation(_property.Name);
            writer.WriteToTranslation(_accessors);
        }
    }
}