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
        private ReadOnlyCollection<GenericParameterExpression> _readonlyGenericParameters;
        private readonly List<MethodExpression> _methodExpressions;
        private ReadOnlyCollection<MethodExpression> _readOnlyMethodExpressions;
        private Type _type;
        private List<Type> _interfaceTypes;
        private ReadOnlyCollection<Type> _readOnlyInterfaceTypes;

        internal TypeExpression(SourceCodeExpression sourceCode, string name)
        {
            SourceCode = sourceCode;
            Name = name.ThrowIfInvalidName<ArgumentException>("Type");
            _methodExpressions = new List<MethodExpression>();
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
        public override Type Type => _type ??= CreateType();

        #region Type Creation

        private Type CreateType()
        {
            var sourceCode = new SourceCodeExpression(SourceCode.Namespace);
            sourceCode.Add(this);

            var compiledTypes = sourceCode
                .Compile()
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
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
                return _readonlyGenericParameters ??= IsGeneric
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
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="TypeExpression"/>'s
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

            ThrowIfInterfaceMethodNotImplemented(interfaceMethods);
            ThrowIfAmbiguousImplementation(interfaceMethods);
        }

        private void ThrowIfInterfaceMethodNotImplemented(IEnumerable<MethodInfo> interfaceMethods)
        {
            var unimplementedMethod = interfaceMethods.FirstOrDefault(method =>
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

        void ITypeExpressionConfigurator.SetImplements(params Type[] interfaces)
            => SetImplements(interfaces);

        void ITypeExpressionConfigurator.SetImplements(
            Type @interface,
            Action<IImplementationConfigurator> configuration)
        {
            var configurator = new ImplementationConfigurator(@interface);
            configuration.Invoke(configurator);
            SetImplements(configurator.GetImplementedType());

            GenericArguments ??= new List<ClosedGenericTypeArgumentExpression>();
            GenericArguments.Add(configurator.GenericArgumentExpression);
        }

        internal void SetImplements(params Type[] interfaces)
        {
            if (_interfaceTypes == null)
            {
                _interfaceTypes = new List<Type>();
            }
            else
            {
                _readOnlyInterfaceTypes = null;
            }

            foreach (var @interface in interfaces)
            {
                ThrowIfNonInterfaceType(@interface);
                _interfaceTypes.Add(@interface);
            }
        }

        private static void ThrowIfNonInterfaceType(Type type)
        {
            if (!type.IsInterface())
            {
                throw new InvalidOperationException(
                    $"Type '{type.GetFriendlyName()}' is not an interface type.");
            }
        }

        void ITypeExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void ITypeExpressionConfigurator.SetVisibility(TypeVisibility visibility)
            => Visibility = visibility;

        GenericParameterExpression IGenericParameterConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            return AddGenericParameter(new OpenGenericArgumentExpression(name, configuration));
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
            _readonlyGenericParameters = null;
            return parameter;
        }

        internal MethodExpression AddMethod(string name, Expression body)
            => AddMethod(name, cfg => cfg.SetBody(body));

        internal MethodExpression AddMethod(
            string name,
            Action<MethodExpression> configuration)
        {
            return Add(new StandardMethodExpression(this, name, configuration));
        }

        internal virtual StandardMethodExpression Add(StandardMethodExpression method)
        {
            Add((MethodExpression)method);

            if (!method.HasBlockMethods)
            {
                return method;
            }

            foreach (var blockMethod in method.BlockMethods)
            {
                blockMethod.Finalise();
                Add(blockMethod);
            }

            return method;
        }

        /// <summary>
        /// Adds the given <paramref name="method"/> to this <see cref="TypeExpression"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodExpression"/> to add.</param>
        /// <returns>The given <paramref name="method"/>.</returns>
        protected internal MethodExpression Add(MethodExpression method)
        {
            _methodExpressions.Add(method);
            _readOnlyMethodExpressions = null;

            if (_type != null)
            {
                _type = CreateType();
            }

            return method;
        }

        #endregion

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => GetTranslation(context);

        internal abstract ITranslation GetTranslation(ITranslationContext context);
    }

    internal class ImplementationConfigurator : IImplementationConfigurator
    {
        private readonly Type _implementedType;
        private readonly Type[] _genericTypeArguments;

        public ImplementationConfigurator(Type implementedType)
        {
            _implementedType = implementedType;
            _genericTypeArguments = implementedType.GetGenericTypeArguments();
        }

        public Type GetImplementedType()
            => _implementedType.MakeGenericType(_genericTypeArguments);

        public ClosedGenericTypeArgumentExpression GenericArgumentExpression
        {
            get;
            private set;
        }

        public void SetGenericArgument(GenericParameterExpression parameter, Type type)
        {
            for (var i = 0; i < _genericTypeArguments.Length; ++i)
            {
                if (_genericTypeArguments[i].Name != parameter.Name)
                {
                    continue;
                }

                _genericTypeArguments[i] = type;
                GenericArgumentExpression = ((OpenGenericArgumentExpression)parameter).Close(type);
                return;
            }
        }
    }
}