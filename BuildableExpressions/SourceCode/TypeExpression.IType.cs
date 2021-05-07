namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using BuildableExpressions.Extensions;
    using Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    public partial class TypeExpression : IType
    {
        private readonly Type _bclType;
        private IType _declaringType;
        private ReadOnlyCollection<IType> _readOnlyConstraintTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExpression"/> class for the given
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The Type represented by the <see cref="TypeExpression"/>.</param>
        protected TypeExpression(Type type)
            : this(new SourceCodeExpression(type.Namespace))
        {
            Name = GetTypedExpressionName(type);

            if (type.IsGenericType())
            {
                var argumentTypes = type.GetGenericTypeArguments();
#if NETSTANDARD
                var typeInfo = type.GetTypeInfo();
#else
                var typeInfo = type;
#endif
                var parameterTypes = typeInfo.IsGenericTypeDefinition
                    ? argumentTypes
                    : typeInfo.GetGenericTypeDefinition().GetGenericTypeArguments();

                if (typeInfo.IsGenericParameter)
                {
                    Constraints = typeInfo.GenericParameterAttributes;

                    ConstraintTypes = typeInfo
                        .GetGenericParameterConstraints()
                        .ProjectToArray(TypeExpressionFactory.Create)
                        .ToReadOnlyCollection();
                }

                for (var i = 0; i < argumentTypes.Length; ++i)
                {
                    var parameterType = parameterTypes[i];
                    var parameter = new TypedGenericParameterExpression(parameterType);

                    AddGenericParameter(parameter);

                    var argumentType = argumentTypes[i];

                    if (argumentType == parameterType)
                    {
                        continue;
                    }

                    _genericArguments[i] = TypeExpressionFactory.Create(argumentType);
                }
            }

            _type = _bclType = type;
        }

        #region Setup

        private static string GetTypedExpressionName(Type type)
        {
            var name = type.GetFriendlyName();

            return type.IsNested
                ? name.Substring(name.LastIndexOf('.') + 1)
                : name;
        }

        #endregion

        Assembly IType.Assembly => Type.GetAssembly();

        internal Assembly BclTypeAssembly => _bclType?.GetAssembly();

        string IType.Namespace => SourceCode.Namespace;

        IType IType.BaseType => null;

        ReadOnlyCollection<IType> IType.AllInterfaces
            => _readOnlyAllInterfaces ??= GetAllInterfaces().ToReadOnlyCollection();

        private IList<IType> GetAllInterfaces()
        {
            if (_interfaceExpressions == null)
            {
                return null;
            }

            // TODO:
            return new List<IType>();
        }

        string IType.FullName => null;

        string IType.Name
            => IsGeneric ? $"{Name}`{_genericParameters.Count}" : Name;

        bool IType.IsInterface => IsInterface;

        internal virtual bool IsInterface => false;

        bool IType.IsClass => IsClass;

        internal virtual bool IsClass => false;

        bool IType.IsEnum => IsEnum;

        internal virtual bool IsEnum => false;

        bool IType.IsPrimitive => false;

        bool IType.IsAnonymous => false;

        bool IType.IsAbstract => false;

        bool IType.IsSealed => false;

        bool IType.IsEnumerable => false;

        bool IType.IsDictionary => false;

        bool IType.IsGenericParameter => false;

        bool IType.IsGenericDefinition => false;

        IType IType.GenericDefinition => null;

        int IType.GenericParameterCount => _genericArguments?.Count ?? 0;

        ReadOnlyCollection<IType> IType.GenericTypeArguments
        {
            get
            {
                return _readOnlyGenericArguments ??= _genericArguments
                    .ProjectToArray<TypeExpression, IType>(arg => arg)
                    .ToReadOnlyCollection();
            }
        }

        GenericParameterAttributes IType.Constraints => Constraints;

        /// <summary>
        /// Gets the GenericParameterAttributes describing this <see cref="TypeExpression"/>'s
        /// constraints, if this TypeExpression represents an open generic parameter.
        /// </summary>
        protected virtual GenericParameterAttributes Constraints { get; }

        ReadOnlyCollection<IType> IType.ConstraintTypes
        {
            get
            {
                return _readOnlyConstraintTypes ??=
                    ConstraintTypes
                        .ProjectToArray<TypeExpression, IType>(t => t)
                        .ToReadOnlyCollection() ??
                    Enumerable<IType>.EmptyReadOnlyCollection;
            }
        }

        /// <summary>
        /// Gets the <see cref="TypeExpression"/>s to which this <see cref="TypeExpression"/> is
        /// constrained, if this <see cref="TypeExpression"/> represents an open generic parameter
        /// Type. Defaults to an empty ReadOnlyCollection.
        /// </summary>
        protected virtual ReadOnlyCollection<TypeExpression> ConstraintTypes { get; }

        bool IType.IsNested => false;

        IType IType.DeclaringType
            => _declaringType ??= ClrTypeWrapper.For(_type?.DeclaringType);

        bool IType.IsArray => false;

        IType IType.ElementType => null;

        bool IType.IsObjectType => false;

        bool IType.IsNullable => false;

        IType IType.NonNullableUnderlyingType => null;

        bool IType.IsByRef => false;

        IEnumerable<IMember> IType.AllMembers => _members ??= _memberExpressions
            .ProjectToArray<MemberExpression, IMember>(m => m);

        IEnumerable<IMember> IType.GetMembers(Action<MemberSelector> selectionConfiguator)
            => ((IType)this).AllMembers.Select(selectionConfiguator);

        IEnumerable<TMember> IType.GetMembers<TMember>(Action<MemberSelector> selectionConfiguator)
            => ((IType)this).GetMembers(selectionConfiguator).OfType<TMember>();

        bool IType.Equals(IType otherType)
            => ClrTypeWrapper.AreEqual(this, otherType);

        Type IType.AsType() => Type;
    }
}
