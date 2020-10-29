namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Formatting;

    internal class InstanceExpression : Expression, ICustomTranslationExpression
    {
        private readonly ConcreteTypeExpression _instance;
        private readonly string _keyword;
        private ITranslation _translation;

        public InstanceExpression(
            ConcreteTypeExpression instance,
            string keyword)
        {
            _instance = instance;
            _keyword = keyword;
        }

        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.ThisInstance;

        public override Type Type => _instance.Type;

        protected override Expression Accept(ExpressionVisitor visitor) => this;

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            return _translation ??= new FixedValueTranslation(
                NodeType,
                _keyword,
                Type,
                TokenType.Keyword,
                context.Settings);
        }
    }
}