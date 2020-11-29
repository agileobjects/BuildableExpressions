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
#if NETSTANDARD
                var typeInfo = type.GetTypeInfo();
                var constraints = typeInfo.GenericParameterAttributes;
                var constraintTypes = typeInfo.GetGenericParameterConstraints();
                var isTypeDefinition = typeInfo.IsGenericTypeDefinition;
#else
                var constraints = type.GenericParameterAttributes;
                var constraintTypes = type.GetGenericParameterConstraints();
                var isTypeDefinition = type.IsGenericTypeDefinition;
#endif
                if (constraints != GenericParameterAttributes.None)
                {
                    Constraints = constraints;

                    ConstraintTypes = constraintTypes
                        .ProjectToArray(TypeExpressionFactory.Create)
                        .ToReadOnlyCollection();
                }

                var typeDefinition = isTypeDefinition
                    ? type : type.GetGenericTypeDefinition();

                var parameterTypes = typeDefinition.GetGenericTypeArguments();
                var argumentTypes = type.GetGenericTypeArguments();

                for (var i = 0; i < argumentTypes.Length; ++i)
                {
                    var parameterType = parameterTypes[i];
                    var parameter = new TypedOpenGenericParameterExpression(parameterType);

                    AddGenericParameter(parameter);

                    var argumentType = argumentTypes[i];

                    if (argumentType == parameterType)
                    {
                        continue;
                    }

                    var closedType = TypeExpressionFactory.Create(argumentType);
                    var argument = new GenericArgumentExpression(parameter, closedType);
                    _genericArguments[i] = argument;
                }
            }

            var allMethodInfos = type
                .GetPublicInstanceMethods()
                .Concat(type.GetPublicStaticMethods())
                .Concat(type.GetNonPublicInstanceMethods().Filter(m => m.IsVirtual));

            foreach (var methodInfo in allMethodInfos)
            {
                AddMethod(new MethodInfoMethodExpression(this, methodInfo));
            }

            _type = type;
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
            => _readOnlyGenericArguments ??= _genericArguments.ToReadOnlyCollection();

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
            => _declaringType ??= BclTypeWrapper.For(_type?.DeclaringType);

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
            => BclTypeWrapper.AreEqual(this, otherType);

        Type IType.AsType() => Type;
    }
}
