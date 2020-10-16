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
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents an open class or method generic argument.
    /// </summary>
    public class GenericParameterExpression :
        Expression,
        IGenericParameterNamingContext,
        IGenericParameterExpressionConfigurator,
        IGenericArgument,
        ICustomTranslationExpression
    {
        private bool _hasConstraints;
        private bool _hasStructConstraint;
        private bool _hasClassConstraint;
        private bool _hasNewableConstraint;
        private List<Type> _typeConstraints;
        private ReadOnlyCollection<Type> _readonlyTypeConstraints;

        internal GenericParameterExpression(
            string name,
            Func<IGenericParameterExpressionConfigurator, IGenericParameterExpressionConfigurator> configuration)
        {
            Name = name.ThrowIfInvalidName<ArgumentException>("Generic Parameter");

            configuration.Invoke(this);

            Type = CreateType();
        }

        #region Setup

        private Type CreateType()
        {
            var paramSourceCode = SourceCodeFactory.Default.CreateSourceCode(sc => sc
                .WithNamespace(BuildConstants.GenericParameterTypeNamespace));

            var paramClass = paramSourceCode.AddClass(cls => cls.Named(Name));

            if (_hasStructConstraint)
            {
                paramClass.IsValueType = true;
            }

            IList<Assembly> referenceAssemblies;

            if (_typeConstraints != null)
            {
                referenceAssemblies = new List<Assembly>();

                foreach (var type in _typeConstraints)
                {
#if NETFRAMEWORK
                    AddAssembliesFor(type, referenceAssemblies);
#else
                    AddAssemblyFor(type, referenceAssemblies);
#endif
                    if (type.IsInterface())
                    {
                        paramClass.Implement(type);
                        AddDefaultImplementations(paramClass, type);
                        continue;
                    }

                    paramClass.BaseType = type;

                    if (type.IsAbstract())
                    {
                        paramClass.IsAbstract = true;
                    }
                }
            }
            else
            {
                referenceAssemblies = Enumerable<Assembly>.EmptyArray;
            }

            var compiledTypes = Compiler.Instance
                .Compile(referenceAssemblies, paramSourceCode)
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
        }

#if NETFRAMEWORK
        private static void AddAssembliesFor(Type type, ICollection<Assembly> assemblies)
        {
            while (true)
            {
                AddAssemblyFor(type, assemblies);

                // ReSharper disable once PossibleNullReferenceException
                var baseType = type.BaseType;

                while (baseType != null && baseType != typeof(object))
                {
                    AddAssemblyFor(baseType, assemblies);
                    baseType = baseType.BaseType;
                }

                if (type.IsNested)
                {
                    type = type.DeclaringType;
                    continue;
                }

                return;
            }
        }
#endif
        private static void AddAssemblyFor(Type type, ICollection<Assembly> assemblies)
        {
            var assembly = type.GetAssembly();

            if (!assemblies.Contains(assembly))
            {
                assemblies.Add(assembly);
            }
        }

        private static void AddDefaultImplementations(ClassExpression @class, Type type)
        {
            var methods = type
                .GetPublicInstanceMethods()
                .Concat(type.GetNonPublicInstanceMethods())
                .Filter(m => m.IsAbstract);

            foreach (var method in methods)
            {
                var parameters = method
                    .GetParameters()
                    .ProjectToArray(p => Parameter(p.ParameterType, p.Name));

                var methodLambda = Default(method.ReturnType).ToLambdaExpression(parameters);

                @class.AddMethod(methodLambda, m => m.Named(method.Name));
            }
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        /// <summary>
        /// Gets the type of this <see cref="GenericParameterExpression"/>, which is 'void', as this
        /// class represents an open generic argument.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Visits this <see cref="GenericParameterExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="GenericParameterExpression"/>.
        /// </param>
        /// <returns>This <see cref="GenericParameterExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets this <see cref="GenericParameterExpression"/>'s parent <see cref="MethodExpression"/>.
        /// </summary>
        public MethodExpression Method { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="GenericParameterExpression"/>
        /// </summary>
        public string Name { get; }

        #region IGenericParameterNamingContext Members

        int IGenericParameterNamingContext.Index => Method?.GenericArguments.IndexOf(this) ?? 0;

        #endregion

        #region IGenericParameterExpressionConfigurator Members

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithStructConstraint()
        {
            ThrowIfHasClassConstraint(conflictingConstraint: "struct");
            _hasConstraints = _hasStructConstraint = true;
            return this;
        }

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithClassConstraint()
        {
            ThrowIfHasStructConstraint(conflictingConstraint: "class");
            _hasConstraints = _hasClassConstraint = true;
            return this;
        }

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithNewableConstraint()
        {
            ThrowIfHasStructConstraint(conflictingConstraint: "new()");
            _hasConstraints = _hasNewableConstraint = true;
            return this;
        }

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithTypeConstraint<T>()
            => AddTypeConstraints(typeof(T));

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithTypeConstraint(Type type)
            => AddTypeConstraints(type);

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithTypeConstraints(params Type[] types)
            => AddTypeConstraints(types);

        private IGenericParameterExpressionConfigurator AddTypeConstraints(params Type[] types)
        {
            _typeConstraints ??= new List<Type>();

            foreach (var type in types)
            {
                if (type.IsClass())
                {
                    var typeName = type.GetFriendlyName();

                    ThrowIfHasStructConstraint(conflictingConstraint: typeName);
                    ThrowIfHasClassConstraint(conflictingConstraint: typeName);
                    ThrowIfAlreadyHasTypeConstraint(conflictingTypeConstraint: typeName);
                }

                _typeConstraints.Add(type);
            }

            _hasConstraints = true;
            _readonlyTypeConstraints = null;
            return this;
        }

        private void ThrowIfHasStructConstraint(string conflictingConstraint)
        {
            if (_hasStructConstraint)
            {
                ThrowConstraintConflict("struct", conflictingConstraint);
            }
        }

        private void ThrowIfHasClassConstraint(string conflictingConstraint)
        {
            if (_hasClassConstraint)
            {
                ThrowConstraintConflict("class", conflictingConstraint);
            }
        }

        private void ThrowIfAlreadyHasTypeConstraint(string conflictingTypeConstraint)
        {
            var existingClassConstraint = _typeConstraints
                .FirstOrDefault(t => t.IsClass());

            if (existingClassConstraint != null)
            {
                ThrowConstraintConflict(
                    existingClassConstraint.GetFriendlyName(),
                    conflictingTypeConstraint);
            }
        }

        private void ThrowConstraintConflict(string constraint, string conflictingConstraint)
        {
            throw new InvalidOperationException(
                $"Generic Parameter '{Name}' cannot have both " +
                $"{constraint} and {conflictingConstraint} constraints.");
        }

        #endregion

        #region IGenericArgument Members

        /// <inheritdoc />
        public bool IsClosed => false;

        bool IGenericArgument.HasConstraints => _hasConstraints;

        bool IGenericArgument.HasClassConstraint => _hasClassConstraint;

        bool IGenericArgument.HasStructConstraint => _hasStructConstraint;

        bool IGenericArgument.HasNewableConstraint => _hasNewableConstraint;

        ReadOnlyCollection<Type> IGenericArgument.TypeConstraints
        {
            get
            {
                return _readonlyTypeConstraints ??=
                       _typeConstraints?.ToReadOnlyCollection() ??
                       Enumerable<Type>.EmptyReadOnlyCollection;
            }
        }

        #endregion

        internal void SetMethod(MethodExpression owningMethod)
        {
            if (Method == null)
            {
                Method = owningMethod;
                return;
            }

            throw new InvalidOperationException(
                 "Unable to add generic parameter to method " +
                $"'{owningMethod.Class.Name}.{owningMethod.Name}' - " +
                 "this parameter has already been added to method " +
                $"'{Method.Class.Name}.{Method.Name}'");
        }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => context.GetTranslationFor(Type);
    }
}