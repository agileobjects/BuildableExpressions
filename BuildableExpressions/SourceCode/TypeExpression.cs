namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using BuildableExpressions.Extensions;
    using Compilation;
    using Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using static SourceCodeTranslationSettings;

    /// <summary>
    /// Represents a Type in a piece of source code.
    /// </summary>
    public abstract class TypeExpression :
        Expression,
        ITypeExpressionConfigurator,
        ICustomTranslationExpression
    {
        private List<GenericParameterExpression> _genericParameters;
        private ReadOnlyCollection<GenericParameterExpression> _readOnlyGenericParameters;
        private List<PropertyExpression> _propertyExpressions;
        private ReadOnlyCollection<PropertyExpression> _readOnlyPropertyExpressions;
        private List<MethodExpression> _methodExpressions;
        private ReadOnlyCollection<MethodExpression> _readOnlyMethodExpressions;
        private Type _type;
        private bool _rebuildType;
        private List<Type> _interfaceTypes;
        private ReadOnlyCollection<Type> _readOnlyInterfaceTypes;
        private ReadOnlyCollection<InterfaceExpression> _readOnlyInterfaceTypeExpressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="TypeExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="TypeExpression"/>.</param>
        protected TypeExpression(SourceCodeExpression sourceCode, string name)
        {
            SourceCode = sourceCode;
            Name = name.ThrowIfInvalidName<ArgumentException>("Type");
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

        internal Type TypeAccessor => _type;

        #region Type Creation

        private Type GetOrCreateType()
        {
            if (!_rebuildType && _type != null)
            {
                return _type;
            }

            _rebuildType = false;

            var sourceCode = new SourceCodeExpression(SourceCode.Namespace);

            foreach (var typeExpression in ImplementedTypeExpressions)
            {
                sourceCode.Add(typeExpression);
            }
            
            sourceCode.Add(this);

            var compiledTypes = sourceCode
                .Compile()
                .CompiledAssembly
                .GetTypes();

            return _type = compiledTypes[0];
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
            => _readOnlyInterfaceTypeExpressions ??= GetInterfaceExpressions().ToReadOnlyCollection();

        #region InterfaceExpression Creation

        private IList<InterfaceExpression> GetInterfaceExpressions()
        {
            return InterfaceTypes.ProjectToArray(@interface =>
            {
                var matchingTypeExpression = SourceCode
                    .TypeExpressions
                    .FirstOrDefault(type => type.TypeAccessor == @interface);

                if (matchingTypeExpression != null)
                {
                    return (InterfaceExpression)matchingTypeExpression;
                }

                return new TypedInterfaceExpression(SourceCode, @interface);
            });
        }

        #endregion

        /// <summary>
        /// Gets the interface types implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<Type> InterfaceTypes
            => _readOnlyInterfaceTypes ??= _interfaceTypes.ToReadOnlyCollection();

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

        internal ICollection<ClosedGenericTypeArgumentExpression> GenericArguments
        {
            get;
            private set;
        }

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
            => _readOnlyMethodExpressions ??= _methodExpressions.ToReadOnlyCollection();

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

        #region ITypeExpressionConfigurator Members

        internal virtual void SetImplements(
            Type @interface,
            Action<ImplementationConfigurator> configuration)
        {
            @interface.ThrowIfNull(nameof(@interface));
            ThrowIfNonInterfaceType(@interface);

            if (configuration == null)
            {
                SetImplements(@interface);
                return;
            }

            var configurator = new ImplementationConfigurator(this, @interface);
            configuration.Invoke(configurator);

            @interface = configurator.GetImplementedType();
            SetImplements(@interface);
        }

        private static void ThrowIfNonInterfaceType(Type type)
        {
            if (!type.IsInterface())
            {
                throw new ArgumentException(
                    $"Type '{type.GetFriendlyName()}' is not an interface type.");
            }
        }

        /// <summary>
        /// Throws an InvalidOperationException if this <see cref="TypeExpression"/> has not
        /// implemented any of the given <paramref name="interface"/>'s methods.
        /// </summary>
        /// <param name="interface">The type the implementation of which should be validated.</param>
        protected void ThrowIfInterfaceMethodNotImplemented(Type @interface)
        {
            var unimplementedMethod = new[] { @interface }
                .Concat(@interface.GetAllInterfaces())
                .SelectMany(it => it.GetPublicMethods())
                .FirstOrDefault(method =>
                {
                    var implementationExists = MethodExpressions.Any(m =>
                        m.Name == method.Name &&
                        m.Parameters.Project(p => p.Type)
                            .SequenceEqual(method.GetParameters().Project(p => p.ParameterType)));

                    return !implementationExists;
                });

            if (unimplementedMethod == null)
            {
                return;
            }

            var wrapper = new BclMethodWrapper(unimplementedMethod, Settings);

            throw new InvalidOperationException(
                $"Method '{wrapper.GetSignature()}' has not been implemented");
        }

        internal void SetImplements(Type @interface)
        {
            _interfaceTypes ??= new List<Type>();
            _interfaceTypes.Add(@interface);
            _readOnlyInterfaceTypeExpressions = null;
            _readOnlyInterfaceTypes = null;
        }

        void ITypeExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void ITypeExpressionConfigurator.SetVisibility(TypeVisibility visibility)
            => Visibility = visibility;

        GenericParameterExpression IGenericParameterConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            return AddGenericParameter(new ConfiguredOpenGenericArgumentExpression(name, configuration));
        }

        /// <summary>
        /// Adds the given <paramref name="parameter"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        /// <returns>The given <paramref name="parameter"/>.</returns>
        protected GenericParameterExpression AddGenericParameter(
            GenericParameterExpression parameter)
        {
            _genericParameters ??= new List<GenericParameterExpression>();
            _genericParameters.Add(parameter);
            _readOnlyGenericParameters = null;
            ResetTypeIfRequired();
            return parameter;
        }

        internal void AddGenericArgument(ClosedGenericTypeArgumentExpression argument)
        {
            GenericArguments ??= new List<ClosedGenericTypeArgumentExpression>();
            GenericArguments.Add(argument);
            ResetTypeIfRequired();
        }

        internal PropertyExpression AddProperty(
            string name,
            Type type,
            Action<PropertyExpression> configuration)
        {
            return AddProperty(new PropertyExpression(this, name, type, configuration));
        }

        /// <summary>
        /// Adds the given <paramref name="property"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="property">The <see cref="PropertyExpression"/> to add.</param>
        /// <returns>The given <paramref name="property"/>.</returns>
        protected PropertyExpression AddProperty(PropertyExpression property)
        {
            _propertyExpressions ??= new List<PropertyExpression>();
            _propertyExpressions.Add(property);
            _readOnlyPropertyExpressions = null;
            ResetTypeIfRequired();
            return property;
        }

        internal MethodExpression AddMethod(string name, Expression body)
            => AddMethod(name, cfg => cfg.SetBody(body));

        internal MethodExpression AddMethod(
            string name,
            Action<MethodExpression> configuration)
        {
            return AddMethod(new StandardMethodExpression(this, name, configuration));
        }

        internal virtual StandardMethodExpression AddMethod(
            StandardMethodExpression method)
        {
            AddMethod((MethodExpression)method);

            if (!method.HasBlockMethods)
            {
                return method;
            }

            foreach (var blockMethod in method.BlockMethods)
            {
                blockMethod.Finalise();
                AddMethod(blockMethod);
            }

            return method;
        }

        /// <summary>
        /// Adds the given <paramref name="method"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodExpression"/> to add.</param>
        /// <returns>The given <paramref name="method"/>.</returns>
        protected MethodExpression AddMethod(MethodExpression method)
        {
            _methodExpressions ??= new List<MethodExpression>();
            _methodExpressions.Add(method);
            _readOnlyMethodExpressions = null;
            ResetTypeIfRequired();
            return method;
        }

        private void ResetTypeIfRequired()
        {
            if (_type != null)
            {
                _rebuildType = true;
                //   _type = CreateType();
            }
        }

        #endregion

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
    }
}