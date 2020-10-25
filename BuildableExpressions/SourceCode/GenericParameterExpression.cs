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
        ICustomTranslationExpression,
        IEquatable<GenericParameterExpression>
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
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            Name = name.ThrowIfInvalidName<ArgumentException>("Generic Parameter");

            configuration.Invoke(this);
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        /// <summary>
        /// Gets the type of this <see cref="GenericParameterExpression"/>, which is lazily,
        /// dynamically created using this parameter's constraints. Different
        /// <see cref="GenericParameterExpression"/>s with the same constraints will have the same
        /// Type.
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
                    sc.AddStruct(parameter.Name, parameter.ConfigureStruct);
                }
                else
                {
                    sc.AddClass(parameter.Name, parameter.ConfigureClass);
                }
            });

            var compiledTypes = paramSourceCode
                .Compile()
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
        }

        private void ConfigureStruct(IStructExpressionConfigurator structConfig)
            => ConfigureType((StructExpression)structConfig, baseTypeCallback: null);

        private void ConfigureClass(IClassExpressionConfigurator classConfig)
        {
            var @class = (ClassExpression)classConfig;

            ConfigureType(@class, (cfg, baseType) =>
            {
                cfg.SetBaseType(baseType);

                if (baseType.IsAbstract())
                {
                    cfg.SetAbstract();
                }
            });
        }

        private void ConfigureType<TTypeExpression>(
            TTypeExpression typeExpression,
            Action<TTypeExpression, Type> baseTypeCallback)
            where TTypeExpression : TypeExpression
        {
            if (_typeConstraints == null)
            {
                return;
            }

            foreach (var type in _typeConstraints)
            {
                if (type.IsInterface())
                {
                    typeExpression.SetImplements(type);
                    AddDefaultImplementations(typeExpression, type);
                    continue;
                }

                baseTypeCallback.Invoke(typeExpression, type);
            }
        }

        private static void AddDefaultImplementations(
            TypeExpression typeExpression,
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

                typeExpression.AddMethod(method.Name, methodLambda);
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
                if (type.IsInterface())
                {
                    _typeConstraints.Add(type);
                    continue;
                }

                var typeName = type.GetFriendlyName();

                ThrowIfHasStructConstraint(conflictingConstraint: typeName);
                ThrowIfHasClassConstraint(conflictingConstraint: typeName);
                ThrowIfAlreadyHasClassConstraint(conflictingTypeConstraint: typeName);

                _typeConstraints.Insert(0, type);
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

        private void ThrowIfAlreadyHasClassConstraint(string conflictingTypeConstraint)
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

        bool IEquatable<GenericParameterExpression>.Equals(GenericParameterExpression other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (_hasConstraints != other._hasConstraints ||
                _hasStructConstraint != other._hasStructConstraint ||
                _hasClassConstraint != other._hasClassConstraint ||
                _hasNewableConstraint != other._hasNewableConstraint ||
                _typeConstraints?.Count != other._typeConstraints?.Count ||
                !Name.Equals(other.Name, Ordinal))
            {
                return false;
            }

            if (_typeConstraints == null)
            {
                return true;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return _typeConstraints
                .OrderBy(t => t)
                .SequenceEqual(other._typeConstraints.OrderBy(t => t));
        }

        private class GenericParameterExpressionComparer : IEqualityComparer<GenericParameterExpression>
        {
            public bool Equals(GenericParameterExpression x, GenericParameterExpression y)
                => ((IEquatable<GenericParameterExpression>)x)?.Equals(y) == true;

            public int GetHashCode(GenericParameterExpression obj) => 0;
        }
    }
}