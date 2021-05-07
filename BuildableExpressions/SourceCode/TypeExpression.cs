namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BuildableExpressions.Extensions;
    using Compilation;
    using Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a Type in a piece of source code.
    /// </summary>
    public abstract partial class TypeExpression :
        Expression,
        ICustomTranslationExpression
    {
        private List<GenericParameterExpression> _genericParameters;
        private ReadOnlyCollection<GenericParameterExpression> _readOnlyGenericParameters;
        private List<TypeExpression> _genericArguments;
        private ReadOnlyCollection<IType> _readOnlyGenericArguments;
        private ConstructorExpression _defaultCtorExpression;
        private List<ConstructorExpression> _ctorExpressions;
        private ReadOnlyCollection<ConstructorExpression> _readOnlyCtorExpressions;
        private readonly List<MemberExpression> _memberExpressions;
        private IMember[] _members;
        private ReadOnlyCollection<MemberExpression> _readOnlyMemberExpressions;
        private List<FieldExpression> _fieldExpressions;
        private ReadOnlyCollection<FieldExpression> _readOnlyFieldExpressions;
        private List<PropertyExpression> _propertyExpressions;
        private ReadOnlyCollection<PropertyExpression> _readOnlyPropertyExpressions;
        private List<MethodExpression> _methodExpressions;
        private List<BlockMethodExpression> _blockMethodExpressions;
        private ReadOnlyCollection<MethodExpression> _readOnlyMethodExpressions;
        private Type _type;
        private bool _rebuildType;
        private List<InterfaceExpression> _interfaceExpressions;
        private ReadOnlyCollection<InterfaceExpression> _readOnlyInterfaceExpressions;
        private ReadOnlyCollection<Type> _readOnlyInterfaceTypes;
        private ReadOnlyCollection<IType> _readOnlyAllInterfaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="TypeExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="TypeExpression"/>.</param>
        protected TypeExpression(SourceCodeExpression sourceCode, string name)
            : this(sourceCode)
        {
            Name = name.ThrowIfInvalidName("Type");
        }

        private TypeExpression(SourceCodeExpression sourceCode)
        {
            SourceCode = sourceCode;
            _memberExpressions = new List<MemberExpression>();
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1001) indicating the type of this
        /// <see cref="TypeExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Type;

        /// <summary>
        /// Gets the type of this <see cref="TypeExpression"/>, which is lazily, dynamically created
        /// using this type's definition.
        /// </summary>
        public override Type Type => GetOrCreateType();

        #region Type Creation

        private Type GetOrCreateType()
        {
            if (!_rebuildType && _type != null)
            {
                return _type;
            }

            _rebuildType = false;

            var sourceCode = new SourceCodeExpression(SourceCode.Namespace);

            var configuredDependencies = ImplementedTypeExpressions
                .Filter(t => t is not ITypedTypeExpression);

            foreach (var typeExpression in configuredDependencies)
            {
                sourceCode.Add(typeExpression);
            }

            sourceCode.Add(this);
            sourceCode.Analyse();

            var compilationResult = sourceCode.Compile();

            if (compilationResult.Failed)
            {
                throw new InvalidOperationException(
                    $"Compilation of type '{Name}' failed:{Environment.NewLine}" +
                    string.Join(Environment.NewLine, compilationResult.Errors));
            }

            var compiledTypes = compilationResult
                .CompiledAssembly
                .GetTypes();

            return _type = compiledTypes.First(t =>
            {
                if (t.Name == Name)
                {
                    return true;
                }

                if (_genericParameters == null)
                {
                    return false;
                }

                return t.Name == Name + "`" + _genericParameters.Count;
            });
        }

        #endregion

        /// <summary>
        /// Visits each of this <see cref="TypeExpression"/>'s <see cref="MethodExpressions"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="TypeExpression"/>'s
        /// <see cref="MethodExpressions"/>.
        /// </param>
        /// <returns>This <see cref="TypeExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(Summary);

            foreach (var method in MethodExpressions)
            {
                visitor.Visit(method);
            }

            return this;
        }

        /// <summary>
        /// Gets this <see cref="TypeExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </summary>
        public SourceCodeExpression SourceCode { get; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="TypeExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Gets this <see cref="TypeExpression"/>'s <see cref="TypeVisibility" />.
        /// </summary>
        public TypeVisibility Visibility { get; private set; }

        /// <summary>
        /// Gets the <see cref="TypeExpression"/>s implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public virtual IEnumerable<TypeExpression> ImplementedTypeExpressions
            => InterfaceTypeExpressions;

        /// <summary>
        /// Gets the types implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public virtual IEnumerable<Type> ImplementedTypes => InterfaceTypes;

        /// <summary>
        /// Gets the <see cref="InterfaceExpression"/>s implemented by this
        /// <see cref="TypeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<InterfaceExpression> InterfaceTypeExpressions
            => _readOnlyInterfaceExpressions ??= _interfaceExpressions.ToReadOnlyCollection();

        /// <summary>
        /// Gets the interface types implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<Type> InterfaceTypes
            => _readOnlyInterfaceTypes ??= GetInterfaceTypes().ToReadOnlyCollection();

        #region InterfaceExpression Creation

        private IList<Type> GetInterfaceTypes()
            => _interfaceExpressions?.ProjectToArray(itf => itf.Type);

        #endregion

        /// <summary>
        /// Gets the name of this <see cref="TypeExpression"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TypeExpression"/> is generic.
        /// </summary>
        public bool IsGeneric => _genericParameters?.Any() == true;

        /// <summary>
        /// Gets the <see cref="GenericParameterExpression"/>s describing the open generic arguments
        /// of this <see cref="TypeExpression"/>, if any.
        /// </summary>
        public ReadOnlyCollection<GenericParameterExpression> GenericParameters
        {
            get
            {
                return _readOnlyGenericParameters ??= IsGeneric
                    ? _genericParameters.ToReadOnlyCollection()
                    : Enumerable<GenericParameterExpression>.EmptyReadOnlyCollection;
            }
        }

        /// <summary>
        /// Closes the given <paramref name="genericParameter"/> to the given
        /// <paramref name="closedTypeExpression"/> on the given <paramref name="closedInstance"/>.
        /// </summary>
        /// <param name="closedInstance">
        /// The <see cref="TypeExpression"/> on which the given <paramref name="genericParameter"/>
        /// is being closed.
        /// </param>
        /// <param name="genericParameter">The <see cref="GenericParameterExpression"/> being closed.</param>
        /// <param name="closedTypeExpression">
        /// The <see cref="TypeExpression"/> to which the given <paramref name="genericParameter"/>
        /// is being closed.
        /// </param>
        protected void Close(
            TypeExpression closedInstance,
            GenericParameterExpression genericParameter,
            TypeExpression closedTypeExpression)
        {
            var genericParameterCount = _genericParameters.Count;

            closedInstance._genericParameters = new List<GenericParameterExpression>(genericParameterCount);
            closedInstance._genericArguments = new List<TypeExpression>(genericParameterCount);

            for (var i = 0; i < genericParameterCount; ++i)
            {
                var parameter = _genericParameters[i];

                closedInstance._genericParameters.Add(parameter);

                closedInstance._genericArguments.Add(parameter == genericParameter
                    ? closedTypeExpression : parameter);
            }

            if (_interfaceExpressions != null)
            {
                closedInstance._interfaceExpressions =
                    new List<InterfaceExpression>(_interfaceExpressions);
            }

            closedInstance._memberExpressions.AddRange(_memberExpressions);

            if (_propertyExpressions != null)
            {
                closedInstance._propertyExpressions =
                    new List<PropertyExpression>(_propertyExpressions);
            }

            if (_methodExpressions != null)
            {
                closedInstance._methodExpressions =
                    new List<MethodExpression>(_methodExpressions);
            }
        }

        internal bool TryGetTypeExpressionFor(
            GenericParameterExpression genericParameter,
            out TypeExpression typeExpression)
        {
            var implementedTypeExpressions = ImplementedTypeExpressions
                .Filter(impl => impl._genericParameters != null);

            foreach (var implementedType in implementedTypeExpressions)
            {
                var index = -1;

                foreach (var candidateParameter in implementedType._genericParameters)
                {
                    ++index;

                    if (candidateParameter != genericParameter)
                    {
                        continue;
                    }

                    typeExpression = implementedType._genericArguments[index];
                    return true;
                }
            }

            typeExpression = null;
            return false;
        }

        /// <summary>
        /// Gets the <see cref="ConstructorExpression"/>s which describe this
        /// <see cref="TypeExpression"/>'s constructors.
        /// </summary>
        public ReadOnlyCollection<ConstructorExpression> ConstructorExpressions
            => _readOnlyCtorExpressions ??= _ctorExpressions.ToReadOnlyCollection();

        internal ICollection<ConstructorExpression> ConstructorExpressionsAccessor
            => _ctorExpressions;

        /// <summary>
        /// Gets the <see cref="MemberExpression"/>s which describe this
        /// <see cref="TypeExpression"/>'s constructors, fields, properties and methods.
        /// </summary>
        public ReadOnlyCollection<MemberExpression> MemberExpressions
            => _readOnlyMemberExpressions ??= _memberExpressions.ToReadOnlyCollection();

        internal virtual ICollection<MemberExpression> MemberExpressionsAccessor
            => _memberExpressions;

        /// <summary>
        /// Gets the <see cref="FieldExpression"/>s which describe this
        /// <see cref="TypeExpression"/>'s fields.
        /// </summary>
        public ReadOnlyCollection<FieldExpression> FieldExpressions
            => _readOnlyFieldExpressions ??= _fieldExpressions.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="PropertyExpression"/>s which describe this
        /// <see cref="TypeExpression"/>'s properties.
        /// </summary>
        public ReadOnlyCollection<PropertyExpression> PropertyExpressions
            => _readOnlyPropertyExpressions ??= _propertyExpressions.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which describe this <see cref="TypeExpression"/>'s
        /// methods.
        /// </summary>
        public ReadOnlyCollection<MethodExpression> MethodExpressions
            => _readOnlyMethodExpressions ??= CreateMethodExpressions().ToReadOnlyCollection();

        private IList<MethodExpression> CreateMethodExpressions()
        {
            if (_methodExpressions != null)
            {
                return _methodExpressions;
            }

            if (_bclType != null)
            {
                return _methodExpressions = _bclType
                    .GetPublicInstanceMethods()
                    .Concat(_bclType.GetPublicStaticMethods())
                    .Concat(_bclType.GetNonPublicInstanceMethods().Filter(m => m.IsVirtual))
                    .Project<MethodInfo, MethodExpression>(methodInfo =>
                        new MethodInfoMethodExpression(this, methodInfo))
                    .ToList();
            }

            return null;
        }

        internal ICollection<MethodExpression> MethodExpressionsAccessor => _methodExpressions;

        #region Validation

        /// <summary>
        /// Validates that this <see cref="TypeExpression"/> has a valid state.
        /// </summary>
        protected void Validate()
        {
            ThrowIfDuplicateTypeName();
            ThrowIfInvalidImplementations();
        }

        private void ThrowIfDuplicateTypeName()
        {
            var hasDuplicateName = SourceCode.TypeExpressions
                .Any(t => t != this && t.Name == Name);

            if (hasDuplicateName)
            {
                throw new InvalidOperationException(
                    $"Duplicate type name '{Name}' specified.");
            }
        }

        private void ThrowIfInvalidImplementations()
        {
            if (InterfaceTypes.Count == 0)
            {
                return;
            }

            var interfaceMethods = InterfaceTypes
                .SelectMany(type => new[] { type }
                    .Concat(type.GetAllInterfaces())
                    .SelectMany(it => it.GetPublicMethods()))
                .ToList();

            ThrowIfAmbiguousImplementation(interfaceMethods);
        }

        private void ThrowIfAmbiguousImplementation(IEnumerable<MethodInfo> interfaceMethods)
        {
            var ambiguousMethod = MethodExpressions.FirstOrDefault(method =>
            {
                var parameterTypes = method.Parameters.ProjectToArray(p => p.Type);

                var matchingInterfaceMethodCount = interfaceMethods.Count(im =>
                    im.Name == method.Name &&
                    im.GetParameters().Project(p => p.ParameterType).SequenceEqual(parameterTypes));

                return matchingInterfaceMethodCount > 1;
            });

            if (ambiguousMethod == null)
            {
                return;
            }

            throw new AmbiguousMatchException(
                $"Method '{ambiguousMethod.GetSignature()}' matches multiple interface methods");
        }

        #endregion

        internal void Finalise()
        {
            if (SourceCode.IsComplete)
            {
                RemoveDefaultConstructorIfPresent();
            }

            if (_blockMethodExpressions != null)
            {
                foreach (var blockMethod in _blockMethodExpressions)
                {
                    blockMethod.Finalise();
                    AddMethod((MethodExpression)blockMethod);
                }

                _blockMethodExpressions = null;
            }

            _memberExpressions.Sort(MemberExpressionComparer.Instance);
        }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => GetTranslation(context);

        /// <summary>
        /// When overridden in a derived class, gets an <see cref="ITranslation"/> with which to
        /// translate this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="context">The ITranslationContext describing the current translation.</param>
        /// <returns>
        /// An <see cref="ITranslation"/> with which to translate this <see cref="TypeExpression"/>.
        /// </returns>
        protected abstract ITranslation GetTranslation(ITranslationContext context);

        /// <summary>
        /// Gets a string representation of this <see cref="TypeExpression"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="TypeExpression"/>.</returns>
        public override string ToString() => Name;
    }
}