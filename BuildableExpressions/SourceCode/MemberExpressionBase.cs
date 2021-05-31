namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using Extensions;
    using ReadableExpressions.Translations.Reflection;
    using static MemberVisibility;

    /// <summary>
    /// Abstract base class for types to which <see cref="AttributeExpression"/>s can be applied.
    /// </summary>
    public abstract class AttributableExpressionBase :
        Expression,
        IAttributableExpressionConfigurator
    {
        private List<AppliedAttribute> _attributes;
        private ReadOnlyCollection<AppliedAttribute> _readOnlyAttributes;

        /// <summary>
        /// Gets the <see cref="AppliedAttribute"/>s describing the
        /// <see cref="AttributeExpression"/>s which have been applied to this
        /// <see cref="AttributableExpressionBase"/>, if any.
        /// </summary>
        public ReadOnlyCollection<AppliedAttribute> Attributes
            => _readOnlyAttributes ??= _attributes.ToReadOnlyCollection();

        internal IList<AppliedAttribute> AttributesAccessor => _attributes;

        #region IAttributableExpressionConfigurator Members

        AppliedAttribute IAttributableExpressionConfigurator.AddAttribute(
            AttributeExpression attribute,
            Action<IAttributeApplicationConfigurator> configuration)
        {
            return AddAttribute(attribute, configuration);
        }

        /// <summary>
        /// Applies the given <paramref name="attribute"/> to this
        /// <see cref="AttributableExpressionBase"/>.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="AttributeExpression"/> to apply to this
        /// <see cref="AttributableExpressionBase"/>.
        /// </param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>
        /// The <see cref="AppliedAttribute"/> describing the application of the given
        /// <paramref name="attribute"/>.
        /// </returns>
        protected AppliedAttribute AddAttribute(
            AttributeExpression attribute,
            Action<IAttributeApplicationConfigurator> configuration)
        {
            var appliedAttribute = new AppliedAttribute(attribute);
            configuration?.Invoke(appliedAttribute);

            _attributes ??= new List<AppliedAttribute>();
            _attributes.Add(appliedAttribute);
            _readOnlyAttributes = null;

            return appliedAttribute;
        }

        #endregion
    }

    /// <summary>
    /// Abstract base class for types describing a member.
    /// </summary>
    public abstract class MemberExpressionBase :
        AttributableExpressionBase,
        IMember
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

        IType IMember.Type => _type ??= ClrTypeWrapper.For(Type);

        bool IMember.IsPublic => Visibility == Public;

        bool IMember.IsProtectedInternal => Visibility == ProtectedInternal;

        bool IMember.IsInternal => Visibility == Internal;

        bool IMember.IsProtected => Visibility == Protected;

        bool IMember.IsPrivateProtected => Visibility == PrivateProtected;

        bool IMember.IsPrivate => Visibility == Private;

        #endregion
    }
}