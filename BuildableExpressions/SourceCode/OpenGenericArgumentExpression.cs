namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Api;
    using BuildableExpressions.Extensions;
    using Compilation;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;

    internal class OpenGenericArgumentExpression :
        GenericParameterExpression,
        IGenericParameterExpressionConfigurator,
        IEquatable<OpenGenericArgumentExpression>
    {
        private static readonly ConcurrentDictionary<OpenGenericArgumentExpression, Type> _typeCache =
            new ConcurrentDictionary<OpenGenericArgumentExpression, Type>(
                new GenericParameterExpressionComparer());

        private Type _type;
        private bool _hasConstraints;
        private bool _hasStructConstraint;
        private bool _hasClassConstraint;
        private bool _hasNewableConstraint;
        private List<Type> _typeConstraints;
        private ReadOnlyCollection<Type> _readonlyTypeConstraints;

        internal OpenGenericArgumentExpression(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
            : base(name.ThrowIfInvalidName<ArgumentException>("Generic Parameter"))
        {
            configuration.Invoke(this);
        }

        public override Type Type
            => _type ??= _typeCache.GetOrAdd(this, CreateType);

        #region Type Creation

        private static Type CreateType(OpenGenericArgumentExpression parameter)
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

        public override bool IsClosed => false;

        public ClosedGenericTypeArgumentExpression Close(Type type)
            => new ClosedGenericTypeArgumentExpression(this, type);

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

        protected override bool HasConstraints => _hasConstraints;

        protected override bool HasClassConstraint => _hasClassConstraint;

        protected override bool HasStructConstraint => _hasStructConstraint;

        protected override bool HasNewableConstraint => _hasNewableConstraint;

        protected override ReadOnlyCollection<Type> TypeConstraints
        {
            get
            {
                return _readonlyTypeConstraints ??=
                    _typeConstraints?.ToReadOnlyCollection() ??
                    Enumerable<Type>.EmptyReadOnlyCollection;
            }
        }

        #endregion

        #region IEquatable Members

        bool IEquatable<OpenGenericArgumentExpression>.Equals(OpenGenericArgumentExpression other)
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
                !Name.Equals(other.Name, StringComparison.Ordinal))
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

        private class GenericParameterExpressionComparer : IEqualityComparer<OpenGenericArgumentExpression>
        {
            public bool Equals(OpenGenericArgumentExpression x, OpenGenericArgumentExpression y)
                => ((IEquatable<OpenGenericArgumentExpression>)x)?.Equals(y) == true;

            public int GetHashCode(OpenGenericArgumentExpression obj) => 0;
        }

        #endregion
    }
}