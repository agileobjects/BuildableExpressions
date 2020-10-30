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

        private InstanceExpression(ConcreteTypeExpression instance, string keyword)
            : this(keyword)
        {
            _instance = instance;
        }

        protected InstanceExpression(string keyword)
        {
            _keyword = keyword;
        }

        #region Factory Methods

        public static InstanceExpression Base(ClassExpression baseInstance)
        {
            return baseInstance != null
                ? new InstanceExpression(baseInstance, "base")
                : BaseObjectInstanceExpression.Instance;
        }

        public static InstanceExpression This(ConcreteTypeExpression instance)
            => new InstanceExpression(instance, "this");

        #endregion

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

        #region Helper Class

        private class BaseObjectInstanceExpression : InstanceExpression
        {
            public static readonly InstanceExpression Instance = new BaseObjectInstanceExpression();

            private BaseObjectInstanceExpression()
                : base("base")
            {
            }

            public override Type Type => typeof(object);
        }

        #endregion
    }
}