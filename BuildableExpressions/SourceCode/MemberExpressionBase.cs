namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations.Reflection;
    using static MemberVisibility;

    /// <summary>
    /// Abstract base class for types describing a member.
    /// </summary>
    public abstract class MemberExpressionBase : Expression, IMember
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberExpressionBase"/> class.
        /// </summary>
        /// <param name="name">The name of this <see cref="MemberExpressionBase"/>.</param>
        protected MemberExpressionBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets this <see cref="MemberExpressionBase"/>'s parent Type.
        /// </summary>
        public abstract Type DeclaringType { get; }

        /// <summary>
        /// Gets the <see cref="MemberVisibility"/> of this <see cref="MemberExpressionBase"/>.
        /// </summary>
        public MemberVisibility? Visibility { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MemberExpressionBase"/> is static.
        /// </summary>
        public bool IsStatic { get; protected set; }

        /// <summary>
        /// Gets the name of this <see cref="MemberExpressionBase"/>.
        /// </summary>
        public virtual string Name { get; }

        #region IMember Members

        bool IMember.IsPublic => Visibility == Public;

        bool IMember.IsProtectedInternal => Visibility == ProtectedInternal;

        bool IMember.IsInternal => Visibility == Internal;

        bool IMember.IsProtected => Visibility == Protected;

        bool IMember.IsPrivate => Visibility == Private;

        #endregion
    }
}