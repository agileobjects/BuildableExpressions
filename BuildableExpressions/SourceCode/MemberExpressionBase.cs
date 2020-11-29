namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Linq.Expressions;
    using ReadableExpressions.Translations.Reflection;
    using static MemberVisibility;

    /// <summary>
    /// Abstract base class for types describing a member.
    /// </summary>
    public abstract class MemberExpressionBase : Expression, IMember
    {
        private IType _type;

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
        public abstract IType DeclaringType { get; }

        /// <summary>
        /// Gets the <see cref="MemberVisibility"/> of this <see cref="MemberExpressionBase"/>.
        /// </summary>
        public MemberVisibility? Visibility { get; private set; }

        /// <summary>
        /// Give this <see cref="MemberExpressionBase"/> the given <paramref name="visibility"/>.
        /// </summary>
        protected void SetVisibility(MemberVisibility visibility)
            => Visibility = visibility;

        /// <summary>
        /// Gets a value indicating whether this <see cref="MemberExpressionBase"/> is static.
        /// </summary>
        public virtual bool IsStatic { get; private set; }

        /// <summary>
        /// Mark this <see cref="MemberExpressionBase"/> as static.
        /// </summary>
        protected void SetStatic() => IsStatic = true;

        /// <summary>
        /// Gets the name of this <see cref="MemberExpressionBase"/>.
        /// </summary>
        public virtual string Name { get; }

        #region IMember Members

        IType IMember.Type => _type ??= BclTypeWrapper.For(Type);

        bool IMember.IsPublic => Visibility == Public;

        bool IMember.IsProtectedInternal => Visibility == ProtectedInternal;

        bool IMember.IsInternal => Visibility == Internal;

        bool IMember.IsProtected => Visibility == Protected;

        bool IMember.IsPrivateProtected => Visibility == PrivateProtected;

        bool IMember.IsPrivate => Visibility == Private;

        #endregion
    }
}