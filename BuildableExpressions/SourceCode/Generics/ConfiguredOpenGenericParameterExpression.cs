namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    internal class ConfiguredOpenGenericParameterExpression :
        OpenGenericParameterExpression,
        IGenericParameterExpressionConfigurator,
        IEquatable<ConfiguredOpenGenericParameterExpression>
    {
        private static readonly ConcurrentDictionary<ConfiguredOpenGenericParameterExpression, Type>
            _typeCache = new ConcurrentDictionary<ConfiguredOpenGenericParameterExpression, Type>(
                new GenericParameterExpressionComparer());

        private Type _bclType;
        private bool _hasConstraints;
        private bool _hasStructConstraint;
        private bool _hasClassConstraint;
        private bool _hasNewableConstraint;
        private List<TypeExpression> _typeConstraints;
        private ReadOnlyCollection<TypeExpression> _readOnlyTypeConstraints;
        private ReadOnlyCollection<IType> _readOnlyTypeConstraintTypes;

        public ConfiguredOpenGenericParameterExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
            : this(sourceCode, name.ThrowIfInvalidName<ArgumentException>("Generic Parameter"))
        {
            configuration.Invoke(this);
        }

        private ConfiguredOpenGenericParameterExpression(
            SourceCodeExpression sourceCode,
            string name)
            : base(sourceCode, name)
        {
        }

        public override Type Type
            => _bclType ??= _typeCache.GetOrAdd(this, p => p.CreateType());

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

        void IGenericParameterExpressionConfigurator.AddTypeConstraints(
            params TypeExpression[] typeExpressions)
        {
            _typeConstraints ??= new List<TypeExpression>();

            foreach (var typeExpression in typeExpressions)
            {
                if (typeExpression.IsInterface)
                {
                    _typeConstraints.Add(typeExpression);
                    continue;
                }

                var typeName = typeExpression.GetFriendlyName();

                ThrowIfHasStructConstraint(conflictingConstraint: typeName);
                ThrowIfHasClassConstraint(conflictingConstraint: typeName);
                ThrowIfAlreadyHasClassConstraint(conflictingTypeConstraint: typeName);

                _typeConstraints.Insert(0, typeExpression);
            }

            _hasConstraints = true;
            _readOnlyTypeConstraints = null;
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
                .FirstOrDefault(t => t.IsClass);

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

        #region IGenericParameter Members

        public override bool HasConstraints => _hasConstraints;

        public override bool HasClassConstraint => _hasClassConstraint;

        public override bool HasStructConstraint => _hasStructConstraint;

        public override bool HasNewableConstraint => _hasNewableConstraint;

        protected override ReadOnlyCollection<TypeExpression> ConstraintTypes
        {
            get
            {
                return _readOnlyTypeConstraints ??=
                    _typeConstraints.ToReadOnlyCollection() ??
                    Enumerable<TypeExpression>.EmptyReadOnlyCollection;
            }
        }

        #endregion

        public override ReadOnlyCollection<IType> TypeConstraints
        {
            get
            {
                return _readOnlyTypeConstraintTypes ??=
                    _typeConstraints
                        .ProjectToArray<TypeExpression, IType>(t => t)
                        .ToReadOnlyCollection() ??
                    Enumerable<IType>.EmptyReadOnlyCollection;
            }
        }

        internal IEnumerable<TypeExpression> TypeConstraintsAccessor => _typeConstraints;

        internal override OpenGenericParameterExpression Clone()
        {
            return new ConfiguredOpenGenericParameterExpression(SourceCode, Name)
            {
                _bclType = _bclType,
                _hasConstraints = _hasConstraints,
                _hasClassConstraint = _hasClassConstraint,
                _hasStructConstraint = _hasStructConstraint,
                _hasNewableConstraint = _hasNewableConstraint,
                _typeConstraints = _typeConstraints,
                _readOnlyTypeConstraints = _readOnlyTypeConstraints
            };
        }

        protected override TypeExpression CreateInstance() => Clone();

        #region IEquatable Members

        bool IEquatable<ConfiguredOpenGenericParameterExpression>.Equals(
            ConfiguredOpenGenericParameterExpression other)
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

        private class GenericParameterExpressionComparer :
            IEqualityComparer<ConfiguredOpenGenericParameterExpression>
        {
            public bool Equals(
                ConfiguredOpenGenericParameterExpression x,
                ConfiguredOpenGenericParameterExpression y)
            {
                return ((IEquatable<ConfiguredOpenGenericParameterExpression>)x)?.Equals(y) == true;
            }

            public int GetHashCode(ConfiguredOpenGenericParameterExpression obj) => 0;
        }

        #endregion
    }
}