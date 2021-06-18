namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations;
    using Translations;

    internal class ConfiguredEnumExpression :
        EnumExpression,
        IEnumExpressionConfigurator
    {
        private int _nextValue;

        public ConfiguredEnumExpression(
            ConfiguredSourceCodeExpression sourceCode,
            string name,
            Action<IEnumExpressionConfigurator> configuration)
            : this(sourceCode, name)
        {
            configuration.Invoke(this);
        }

        private ConfiguredEnumExpression(ConfiguredSourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IEnumExpressionConfigurator Members

        void IEnumExpressionConfigurator.AddMembers(params string[] memberNames)
        {
            foreach (var memberName in memberNames)
            {
                AddMember(memberName, value: null);
            }
        }

        void IEnumExpressionConfigurator.AddMember(string name, int value)
            => AddMember(name, value);

        internal void AddMember(string name, int? value)
        {
            if (value.HasValue)
            {
                _nextValue = value.Value + 1;
            }
            else
            {
                value = _nextValue++;
            }

            base.AddMember(name, value.Value);
        }

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new EnumTranslation(this, context);
    }
}