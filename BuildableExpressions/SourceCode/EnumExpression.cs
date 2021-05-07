namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents an enum in a piece of source code.
    /// </summary>
    public abstract class EnumExpression : TypeExpression, IType
    {
        private readonly Dictionary<string, int> _members;
        private ReadOnlyDictionary<string, int> _readOnlyMembers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumExpression"/> class for the given
        /// <paramref name="enumType"/>.
        /// </summary>
        /// <param name="enumType">The Type represented by the <see cref="EnumExpression"/>.</param>
        protected EnumExpression(Type enumType)
            : base(enumType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="EnumExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="EnumExpression"/>.</param>
        protected EnumExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
            _members = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1002) indicating the type of this
        /// <see cref="MethodExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Enum;

        /// <summary>
        /// Gets the members belonging to this <see cref="EnumExpression"/>.
        /// </summary>
        public ReadOnlyDictionary<string, int> Members
            => _readOnlyMembers ??= new ReadOnlyDictionary<string, int>(_members);

        internal IDictionary<string, int> MembersAccessor => _members;

        internal void AddMember(string name, int value)
        {
            MembersAccessor.Add(name, value);
            _readOnlyMembers = null;
        }

        #region IType Members

        IType IType.BaseType => ClrTypeWrapper.Enum;

        internal override bool IsEnum => true;

        #endregion
    }
}