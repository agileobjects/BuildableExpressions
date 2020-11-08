namespace AgileObjects.BuildableExpressions.SourceCode.Translations
{
    using System;
    using System.Linq.Expressions;
    using AgileObjects.ReadableExpressions.Translations;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class TransientPropertyTranslation : ITranslation
    {
        private readonly PropertyExpression _propertyExpression;
        private readonly ITranslatable _typeNameTranslation;
        private readonly bool _writeModifiers;
        private readonly string _accessibility;
        private readonly string _modifiers;
        private readonly string _accessors;

        public TransientPropertyTranslation(
            PropertyExpression propertyExpression,
            ITranslationContext context)
        {
            _propertyExpression = propertyExpression;
            _typeNameTranslation = context.GetTranslationFor(propertyExpression.Type);

            var translationSize = _typeNameTranslation.TranslationSize + 1;

            _writeModifiers = !(propertyExpression is InterfacePropertyExpression);

            if (_writeModifiers)
            {
                _accessibility = propertyExpression.GetAccessibilityForTranslation();
                _modifiers = propertyExpression.GetModifiersForTranslation();
                translationSize += _accessibility.Length + _modifiers.Length;
            }

            var getter = CreateGetter(propertyExpression.GetterExpression);
            var setter = CreateSetter(propertyExpression.SetterExpression);

            _accessors = "{" + getter + setter + "}";

            TranslationSize =
                translationSize +
                propertyExpression.Name.Length +
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

            var accessorName = accessor == _propertyExpression.GetterExpression ? "get" : "set";

            var accessorTranslation = accessorName + (_propertyExpression.IsAutoProperty
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

        public ExpressionType NodeType => _propertyExpression.NodeType;

        public Type Type => _propertyExpression.Type;

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
            writer.WriteToTranslation(' ');
            writer.WriteToTranslation(_propertyExpression.Name);
            writer.WriteToTranslation(_accessors);
        }
    }
}