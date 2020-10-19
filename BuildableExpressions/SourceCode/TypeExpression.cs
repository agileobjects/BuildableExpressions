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
        private readonly List<MethodExpression> _methods;
        private readonly Dictionary<Type, List<MethodExpression>> _methodsByReturnType;
        private ReadOnlyCollection<MethodExpression> _readOnlyMethods;
        private List<Type> _interfaces;
        private ReadOnlyCollection<Type> _readOnlyInterfaces;
#if FEATURE_READONLYDICTIONARY
        private ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> _readOnlyMethodsByReturnType;
#else
        private IDictionary<Type, ReadOnlyCollection<MethodExpression>> _readOnlyMethodsByReturnType;
#endif

        internal TypeExpression(SourceCodeExpression sourceCode, string name)
        {
            SourceCode = sourceCode;
            Name = name.ThrowIfInvalidName<ArgumentException>("Type");
            _methods = new List<MethodExpression>();
            _methodsByReturnType = new Dictionary<Type, List<MethodExpression>>();
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1001) indicating the type of this
        /// <see cref="TypeExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Type;

        /// <summary>
        /// Gets the type of this <see cref="TypeExpression"/>, which is the return type of the
        /// first of the type's <see cref="Methods"/>, or typeof(void) if this type has no methods.
        /// </summary>
        public override Type Type
            => _methods.FirstOrDefault()?.Type ?? typeof(void);

        /// <summary>
        /// Visits each of this <see cref="TypeExpression"/>'s <see cref="Methods"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="TypeExpression"/>'s
        /// <see cref="Methods"/>.
        /// </param>
        /// <returns>This <see cref="TypeExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(Summary);

            foreach (var method in Methods)
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
        public virtual IEnumerable<Type> ImplementedTypes => Interfaces;

        /// <summary>
        /// Gets the interface types implemented by this <see cref="TypeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<Type> Interfaces
            => _readOnlyInterfaces ??= _interfaces.ToReadOnlyCollection();

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
        public ReadOnlyCollection<MethodExpression> Methods
            => _readOnlyMethods ??= _methods.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="TypeExpression"/>'s
        /// methods, keyed by their return type.
        /// </summary>
#if FEATURE_READONLYDICTIONARY
        public ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> MethodsByReturnType
#else
        public IDictionary<Type, ReadOnlyCollection<MethodExpression>> MethodsByReturnType
#endif
            => _readOnlyMethodsByReturnType ??= GetMethodsByReturnType();

#if FEATURE_READONLYDICTIONARY
        private ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> GetMethodsByReturnType()
#else
        private IDictionary<Type, ReadOnlyCollection<MethodExpression>> GetMethodsByReturnType()
#endif
        {
            var readonlyMethodsByReturnType =
                new Dictionary<Type, ReadOnlyCollection<MethodExpression>>(_methodsByReturnType.Count);

            foreach (var methodAndReturnType in _methodsByReturnType)
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

        internal void Validate()
        {
            ThrowIfDuplicateMethodName();
            ThrowIfInvalidImplementations();
        }

        #region Validate Helpers

        private void ThrowIfDuplicateMethodName()
        {
            if (_methods.Count == 1)
            {
                return;
            }

            var duplicateMethodName = _methods
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
            if (Interfaces.Count == 0)
            {
                return;
            }

            var interfaceMethods = Interfaces
                .SelectMany(type => new[] { type }
                    .Concat(type.GetAllInterfaces())
                    .SelectMany(it => it.GetPublicMethods()))
                .ToList();

            foreach (var method in Methods)
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

        void ITypeExpressionConfigurator.SetImplements<TInterface>()
            => Implement(typeof(TInterface));

        void ITypeExpressionConfigurator.SetImplements(params Type[] interfaces)
            => Implement(interfaces);

        private void Implement(params Type[] interfaces)
        {
            if (_interfaces == null)
            {
                _interfaces = new List<Type>();
            }
            else
            {
                _readOnlyInterfaces = null;
            }

            _interfaces.AddRange(interfaces);
        }

        void ITypeExpressionConfigurator.SetSummary(string summary)
            => Summary = ReadableExpression.Comment(summary);

        void ITypeExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void ITypeExpressionConfigurator.SetVisibility(TypeVisibility visibility)
            => Visibility = visibility;

        MethodExpression ITypeExpressionConfigurator.AddMethod(string name, Expression body)
            => AddMethod(name, body, cfg => { });

        MethodExpression ITypeExpressionConfigurator.AddMethod(
            string name,
            Expression body,
            Action<IMethodExpressionConfigurator> configuration)
        {
            return AddMethod(name, body, configuration);
        }

        private MethodExpression AddMethod(
            string name,
            Expression body,
            Action<IMethodExpressionConfigurator> configuration)
        {
            name.ThrowIfInvalidName<ArgumentException>("Method");

            var method = AddMethodWithoutAnalysis(body, configuration, name);
            MethodExpressionAnalysis.For(method);

            return method;
        }

        internal MethodExpression AddMethodWithoutAnalysis(
            Expression body,
            Action<IMethodExpressionConfigurator> configuration,
            string name = null)
        {
            var method = new MethodExpression(this, name, body);
            configuration.Invoke(method);

            ValidateMethod(method);

            _methods.Add(method);
            _readOnlyMethods = null;

            AddTypedMethod(method);
            _readOnlyMethodsByReturnType = null;
            return method;
        }

        internal virtual void ValidateMethod(MethodExpression method)
        {
            method.Validate();
        }

        private void AddTypedMethod(MethodExpression method)
        {
            if (!_methodsByReturnType.TryGetValue(method.ReturnType, out var typedMethods))
            {
                _methodsByReturnType.Add(
                    method.ReturnType,
                    typedMethods = new List<MethodExpression>());
            }

            typedMethods.Add(method);
        }

        #endregion

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
            => Methods.Cast<ICustomAnalysableExpression>().SelectMany(m => m.Expressions);
    }
}