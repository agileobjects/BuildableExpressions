namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using Compilation;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using static System.StringComparison;

    /// <summary>
    /// Represents an open class or method generic argument.
    /// </summary>
    public class GenericParameterExpression :
        Expression,
        IGenericParameterExpressionConfigurator,
        IGenericArgument,
        ICustomTranslationExpression
    {
        private static readonly ConcurrentDictionary<GenericParameterExpression, Type> _typeCache =
            new ConcurrentDictionary<GenericParameterExpression, Type>(
                new GenericParameterExpressionComparer());

        private Type _type;
        private bool _hasConstraints;
        private bool _hasStructConstraint;
        private bool _hasClassConstraint;
        private bool _hasNewableConstraint;
        private List<Type> _typeConstraints;
        private ReadOnlyCollection<Type> _readonlyTypeConstraints;

        internal GenericParameterExpression(
            MethodExpression method,
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            Name = name.ThrowIfInvalidName<ArgumentException>("Generic Parameter");
            Method = method;

            configuration.Invoke(this);
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        /// <summary>
        /// Gets the type of this <see cref="GenericParameterExpression"/>, which is auto-generated
        /// based on this parameter's constraints.
        /// </summary>
        public override Type Type
            => _type ??= _typeCache.GetOrAdd(this, CreateType);

        #region Type Creation

        private static Type CreateType(GenericParameterExpression parameter)
        {
            var paramSourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace(BuildConstants.GenericParameterTypeNamespace);

                if (parameter._hasStructConstraint)
                {
                    sc.AddStruct(parameter.Name, cfg => parameter
                        .ConfigureType(cfg, baseTypeCallback: null));
                }
                else
                {
                    sc.AddClass(parameter.Name, parameter.ConfigureClass);
                }
            });

            var compiledTypes = Compiler.Instance
                .Compile(paramSourceCode)
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
        }

        private void ConfigureClass(IClassExpressionConfigurator classConfig)
        {
            ConfigureType(classConfig, (cfg, baseType) =>
            {
                cfg.SetBaseType(baseType);

                if (baseType.IsAbstract())
                {
                    cfg.SetAbstract();
                }
            });
        }

        private void ConfigureType<TConfigurator>(
            TConfigurator typeConfig,
            Action<TConfigurator, Type> baseTypeCallback)
            where TConfigurator : ITypeExpressionConfigurator
        {
            if (_typeConstraints == null)
            {
                return;
            }

            foreach (var type in _typeConstraints)
            {
                if (type.IsInterface())
                {
                    typeConfig.SetImplements(type);
                    AddDefaultImplementations(typeConfig, type);
                    continue;
                }

                baseTypeCallback.Invoke(typeConfig, type);
            }
        }

        private static void AddDefaultImplementations(
            ITypeExpressionConfigurator typeConfig,
            Type type)
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

                typeConfig.AddMethod(method.Name, methodLambda);
            }
        }

        #endregion

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
        public MethodExpression Method { get; }

        /// <summary>
        /// Gets the name of this <see cref="GenericParameterExpression"/>
        /// </summary>
        public string Name { get; }

        #region IGenericParameterExpressionConfigurator Members

        void IGenericParameterExpressionConfigurator.AddStructConstraint()
        {
            ThrowIfHasClassConstraint(conflictingConstraint: "struct");
            _hasConstraints = _hasStructConstraint = true;
        }

        void IGenericParameterExpressionConfigurator.AddClassConstraint()
        {
            ThrowIfHasStructConstraint(conflictingConstraint: "class");
            _hasConstraints = _hasClassConstraint = true;
        }

        void IGenericParameterExpressionConfigurator.AddNewableConstraint()
        {
            ThrowIfHasStructConstraint(conflictingConstraint: "new()");
            _hasConstraints = _hasNewableConstraint = true;
        }

        void IGenericParameterExpressionConfigurator.AddTypeConstraints(params Type[] types)
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

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => context.GetTranslationFor(Type);

        private class GenericParameterExpressionComparer : IEqualityComparer<GenericParameterExpression>
        {
            public bool Equals(GenericParameterExpression x, GenericParameterExpression y)
            {
                if (ReferenceEquals(y, null))
                {
                    return ReferenceEquals(x, null);
                }

                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                // ReSharper disable once PossibleNullReferenceException
                if (x._hasConstraints != y._hasConstraints ||
                    x._hasStructConstraint != y._hasStructConstraint ||
                    x._hasClassConstraint != y._hasClassConstraint ||
                    x._hasNewableConstraint != y._hasNewableConstraint ||
                    x._typeConstraints?.Count != y._typeConstraints?.Count ||
                    !x.Name.Equals(y.Name, Ordinal))
                {
                    return false;
                }

                if (x._typeConstraints == null)
                {
                    return true;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                return x._typeConstraints
                    .OrderBy(t => t)
                    .SequenceEqual(y._typeConstraints.OrderBy(t => t));
            }

            public int GetHashCode(GenericParameterExpression obj) => 0;
        }
    }
}