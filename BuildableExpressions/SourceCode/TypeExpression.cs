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
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;

    /// <summary>
    /// Represents a Type in a piece of source code.
    /// </summary>
    public abstract class TypeExpression :
        Expression,
        ITypeExpressionConfigurator,
        ICustomAnalysableExpression
    {
        private readonly List<MethodExpression> _methodExpressions;
        private readonly Dictionary<Type, List<MethodExpression>> _methodExpressionsByReturnType;
        private ReadOnlyCollection<MethodExpression> _readOnlyMethodExpressions;
        private Type _type;
        private List<Type> _interfaceTypes;
        private ReadOnlyCollection<Type> _readOnlyInterfaceTypes;
#if FEATURE_READONLYDICTIONARY
        private ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> _readOnlyMethodExpressionsByReturnType;
#else
        private IDictionary<Type, ReadOnlyCollection<MethodExpression>> _readOnlyMethodExpressionsByReturnType;
#endif

        internal TypeExpression(SourceCodeExpression sourceCode, string name)
        {
            SourceCode = sourceCode;
            Name = name.ThrowIfInvalidName<ArgumentException>("Type");
            _methodExpressions = new List<MethodExpression>();
            _methodExpressionsByReturnType = new Dictionary<Type, List<MethodExpression>>();

            sourceCode.Register(this);
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1001) indicating the type of this
        /// <see cref="TypeExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Type;

        /// <summary>
        /// Gets the type of this <see cref="TypeExpression"/>, which is the return type of the
        /// first of the type's <see cref="MethodExpressions"/>, or typeof(void) if this type has no methods.
        /// </summary>
        public override Type Type => _type ??= CreateType();

        #region Type Creation

        private Type CreateType()
        {
            return _methodExpressions.FirstOrDefault()?.Type ?? typeof(void);
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
        /// Gets this <see cref="TypeExpression"/>'s <see cref="TypeVisibility" />.
        /// </summary>
        public TypeVisibility Visibility { get; private set; }

        /// <summary>
        /// Gets the types implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public virtual IEnumerable<Type> ImplementedTypes => InterfaceTypes;

        /// <summary>
        /// Gets the interface types implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<Type> InterfaceTypes
            => _readOnlyInterfaceTypes ??= _interfaceTypes.ToReadOnlyCollection();

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="TypeExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="TypeExpression"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="TypeExpression"/>'s
        /// methods.
        /// </summary>
        public ReadOnlyCollection<MethodExpression> MethodExpressions
            => _readOnlyMethodExpressions ??= _methodExpressions.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="TypeExpression"/>'s
        /// methods, keyed by their return type.
        /// </summary>
#if FEATURE_READONLYDICTIONARY
        public ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> MethodExpressionsByReturnType
#else
        public IDictionary<Type, ReadOnlyCollection<MethodExpression>> MethodExpressionsByReturnType
#endif
            => _readOnlyMethodExpressionsByReturnType ??= GetMethodsByReturnType();

#if FEATURE_READONLYDICTIONARY
        private ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> GetMethodsByReturnType()
#else
        private IDictionary<Type, ReadOnlyCollection<MethodExpression>> GetMethodsByReturnType()
#endif
        {
            var readonlyMethodsByReturnType =
                new Dictionary<Type, ReadOnlyCollection<MethodExpression>>(_methodExpressionsByReturnType.Count);

            foreach (var methodAndReturnType in _methodExpressionsByReturnType)
            {
                readonlyMethodsByReturnType.Add(
                    methodAndReturnType.Key,
                    methodAndReturnType.Value.ToReadOnlyCollection());
            }

            return readonlyMethodsByReturnType
#if FEATURE_READONLYDICTIONARY
                    .ToReadOnlyDictionary()
#endif
                ;
        }

        #region Validation

        internal void Validate()
        {
            ThrowIfDuplicateMethodName();
            ThrowIfInvalidImplementations();
        }

        private void ThrowIfDuplicateMethodName()
        {
            if (_methodExpressions.Count <= 1)
            {
                return;
            }

            var duplicateMethodName = _methodExpressions
                .GroupBy(m => $"{m.Name}({string.Join(",", m.Parameters.Select(p => p.Type.FullName))})")
                .FirstOrDefault(nameGroup => nameGroup.Count() > 1)?
                .Key;

            if (duplicateMethodName != null)
            {
                throw new InvalidOperationException(
                    $"Class '{Name}': duplicate method name '{duplicateMethodName}' specified.");
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

            foreach (var method in MethodExpressions)
            {
                ThrowIfAmbiguousImplementation(method, interfaceMethods);
            }
        }

        private static void ThrowIfAmbiguousImplementation(
            MethodExpression method,
            IEnumerable<MethodInfo> interfaceMethods)
        {
            var parameterTypes = method.Parameters.ProjectToArray(p => p.Type);

            var matchingInterfaceMethodCount = interfaceMethods
                .Count(im =>
                    im.ReturnType == method.ReturnType &&
                    im.GetParameters().Project(p => p.ParameterType).SequenceEqual(parameterTypes));

            if (matchingInterfaceMethodCount <= 1)
            {
                return;
            }

            var argumentTypes = string.Join(", ",
                parameterTypes.ProjectToArray(p => p.GetFriendlyName()));

            var returnType = method.ReturnType.GetFriendlyName();

            throw new AmbiguousMatchException(
                $"Method '({argumentTypes}): {returnType}' matches multiple interface methods");
        }

        #endregion

        #region ITypeExpressionConfigurator Members

        void ITypeExpressionConfigurator.SetImplements(params Type[] interfaces)
        {
            if (_interfaceTypes == null)
            {
                _interfaceTypes = new List<Type>();
            }
            else
            {
                _readOnlyInterfaceTypes = null;
            }

            _interfaceTypes.AddRange(interfaces);
        }

        void ITypeExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void ITypeExpressionConfigurator.SetVisibility(TypeVisibility visibility)
            => Visibility = visibility;

        MethodExpression ITypeExpressionConfigurator.AddMethod(
            string name,
            Expression body,
            Action<IMethodExpressionConfigurator> configuration)
        {
            name.ThrowIfInvalidName<ArgumentException>("Method");

            var method = AddMethod(body, configuration, name);
            MethodExpressionAnalysis.For(method);

            return method;
        }

        internal MethodExpression AddMethod(
            Expression body,
            Action<IMethodExpressionConfigurator> configuration,
            string name = null)
        {
            var method = new MethodExpression(this, name, body, configuration);

            ValidateMethod(method);
            return method;
        }

        internal virtual void ValidateMethod(MethodExpression method)
        {
            method.Validate();
        }

        internal void Register(MethodExpression method)
        {
            _methodExpressions.Add(method);
            _readOnlyMethodExpressions = null;

            AddTypedMethod(method);
            _readOnlyMethodExpressionsByReturnType = null;
        }

        private void AddTypedMethod(MethodExpression method)
        {
            if (!_methodExpressionsByReturnType.TryGetValue(method.ReturnType, out var typedMethods))
            {
                _methodExpressionsByReturnType.Add(
                    method.ReturnType,
                    typedMethods = new List<MethodExpression>());
            }

            typedMethods.Add(method);
        }

        #endregion

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
            => MethodExpressions.Cast<ICustomAnalysableExpression>().SelectMany(m => m.Expressions);
    }
}