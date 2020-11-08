namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Api;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents an enum in a piece of source code.
    /// </summary>
    public class EnumExpression :
        TypeExpression,
        IEnumExpressionConfigurator
    {
        private readonly Dictionary<string, int> _members;
        private ReadOnlyDictionary<string, int> _readOnlyMembers;
        private int _nextValue;

        internal EnumExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IEnumExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            _members = new Dictionary<string, int>();
            configuration.Invoke(this);
        }

        /// <summary>
        /// Gets the members belonging to this <see cref="EnumExpression"/>.
        /// </summary>
        public ReadOnlyDictionary<string, int> Members
            => _readOnlyMembers ??= new ReadOnlyDictionary<string, int>(_members);

        internal IDictionary<string, int> MembersAccessor => _members;

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

            _members.Add(name, value.Value);
            _readOnlyMembers = null;
        }

        #endregion

        /// <inheritdoc />
        protected override ITranslation GetTranslation(ITranslationContext context)
            => new EnumTranslation(this, context);
    }
}