namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public class ClassExpression :
        Expression,
        IClassNamingContext,
        IClassExpressionConfigurator,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        private readonly SourceCodeTranslationSettings _settings;
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
        private string _name;

        internal ClassExpression(
            SourceCodeExpression sourceCode,
            SourceCodeTranslationSettings settings)
        {
            SourceCode = sourceCode;
            _settings = settings;
            _methods = new List<MethodExpression>();
            _methodsByReturnType = new Dictionary<Type, List<MethodExpression>>();
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1001) indicating the type of this
        /// <see cref="ClassExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Class;

        /// <summary>
        /// Gets the type of this <see cref="ClassExpression"/>, which is the return type of the
        /// first of the class' <see cref="Methods"/>, or typeof(void) if this class has no methods.
        /// </summary>
        public override Type Type
            => _methods.FirstOrDefault()?.Type ?? typeof(void);

        /// <summary>
        /// Visits each of this <see cref="ClassExpression"/>'s <see cref="Methods"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="ClassExpression"/>'s
        /// <see cref="Methods"/>.
        /// </param>
        /// <returns>This <see cref="ClassExpression"/>.</returns>
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
        /// Gets this <see cref="ClassExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </summary>
        public SourceCodeExpression SourceCode { get; }

        /// <summary>
        /// Gets the interface types implemented by this <see cref="ClassExpression"/>.
        /// </summary>
        public ReadOnlyCollection<Type> Interfaces
            => _readOnlyInterfaces ??= _interfaces.ToReadOnlyCollection();

        /// <summary>
        /// Adds the given <paramref name="interfaces"/> to the list of interfaces implemented by
        /// this <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="interfaces">The interface(s) to add.</param>
        public void Implement(params Type[] interfaces)
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

        /// <summary>
        /// Gets this <see cref="ClassExpression"/>'s <see cref="ClassVisibility" />.
        /// </summary>
        public ClassVisibility Visibility { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is static.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="ClassExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="ClassExpression"/>.
        /// </summary>
        public string Name => _name ??= GetName();

        private string GetName()
        {
            return _settings
                .ClassNameFactory
                .Invoke(SourceCode, this)
                .ThrowIfInvalidName<InvalidOperationException>("Class");
        }

        /// <summary>
        /// Adds a new method with the given <paramref name="body"/> to this <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="body">
        /// The expression from which to create the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public MethodExpression AddMethod(Expression body)
            => AddMethod(body, cfg => cfg);

        /// <summary>
        /// Adds a new method with the given <paramref name="body"/> to this <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="body">
        /// The expression from which to create the new <see cref="MethodExpression"/>.
        /// </param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>
        /// .</param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public MethodExpression AddMethod(
            Expression body,
            Func<IMethodExpressionConfigurator, IMethodExpressionConfigurator> configuration)
        {
            var method = AddMethodWithoutAnalysis(body, configuration);
            MethodExpressionAnalysis.For(method, _settings);

            return method;
        }

        internal MethodExpression AddMethodWithoutAnalysis(
            Expression body,
            Func<IMethodExpressionConfigurator, IMethodExpressionConfigurator> configuration)
        {
            var method = new MethodExpression(this, _settings);
            configuration.Invoke(method);

            method.SetBody(body);

            if (IsStatic)
            {
                ((IMethodExpressionConfigurator)method).AsStatic();
            }

            _methods.Add(method);
            _readOnlyMethods = null;

            AddTypedMethod(method);
            _readOnlyMethodsByReturnType = null;
            return method;
        }

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="ClassExpression"/>'s
        /// methods.
        /// </summary>
        public ReadOnlyCollection<MethodExpression> Methods
            => _readOnlyMethods ??= _methods.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="ClassExpression"/>'s
        /// methods, kyed by their return type.
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

        /// <summary>
        /// Gets the index of this <see cref="ClassExpression"/> in the set of generated classes.
        /// </summary>
        public int Index => SourceCode.Classes.IndexOf(this);

        internal void Validate()
        {
            ThrowIfDuplicateMethodName();
        }

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

        #region IClassExpressionConfigurator Members

        IClassExpressionConfigurator IClassExpressionConfigurator.AsStatic()
        {
            IsStatic = true;
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.WithVisibility(
            ClassVisibility visibility)
        {
            Visibility = visibility;
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.Named(
            string name)
        {
            _name = name.ThrowIfInvalidName<ArgumentException>("Class");
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.Implementing<TInterface>()
            where TInterface : class
        {
            Implement(typeof(TInterface));
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.Implementing(
            params Type[] interfaces)
        {
            Implement(interfaces);
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.WithSummary(
            string summary)
        {
            Summary = ReadableExpression.Comment(summary);
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.WithSummary(
            CommentExpression summary)
        {
            Summary = summary;
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.WithMethod(
            Expression body)
        {
            AddMethod(body);
            return this;
        }

        IClassExpressionConfigurator IClassExpressionConfigurator.WithMethod(
            Expression body,
            Func<IMethodExpressionConfigurator, IMethodExpressionConfigurator> configuration)
        {
            AddMethod(body, configuration);
            return this;
        }

        #endregion

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
            => Methods.Cast<ICustomAnalysableExpression>().SelectMany(m => m.Expressions);

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}