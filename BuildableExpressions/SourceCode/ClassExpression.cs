namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public class ClassExpression :
        ConcreteTypeExpression,
        IClassExpressionConfigurator
    {
        private ClassExpression _baseTypeExpression;
        private Expression _baseInstanceExpression;

        internal ClassExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IClassExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            BaseType = typeof(object);
            configuration.Invoke(this);
            Validate();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is static.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is abstract.
        /// </summary>
        public bool IsSealed { get; private set; }

        /// <summary>
        /// Gets the base type from which this <see cref="ClassExpression"/> derives.
        /// </summary>
        public Type BaseType { get; private set; }

        /// <summary>
        /// Gets the non-object base type and interfaces types implemented by this
        /// <see cref="ClassExpression"/>.
        /// </summary>
        public override IEnumerable<Type> ImplementedTypes
        {
            get
            {
                if (BaseType != typeof(object))
                {
                    yield return BaseType;
                }

                foreach (var @interface in InterfaceTypes)
                {
                    yield return @interface;
                }
            }
        }

        #region IClassExpressionConfigurator Members

        void IClassExpressionConfigurator.SetImplements(
            Type @interface,
            Action<IClassImplementationConfigurator> configuration)
        {
            SetImplements(@interface, configuration);
        }

        void IClassExpressionConfigurator.SetStatic()
        {
            ThrowIfAbstract("static");
            ThrowIfSealed("static");
            IsStatic = true;
        }

        void IClassExpressionConfigurator.SetAbstract() => SetAbstract();

        internal void SetAbstract()
        {
            ThrowIfStatic("abstract");
            ThrowIfSealed("abstract");
            IsAbstract = true;
        }

        void IClassExpressionConfigurator.SetSealed()
        {
            ThrowIfStatic("sealed");
            ThrowIfAbstract("sealed");
            IsSealed = true;
        }

        private void ThrowIfStatic(string conflictingModifier)
        {
            if (IsStatic)
            {
                ThrowModifierConflict("static", conflictingModifier);
            }
        }

        private void ThrowIfAbstract(string conflictingModifier)
        {
            if (IsAbstract)
            {
                ThrowModifierConflict("abstract", conflictingModifier);
            }
        }

        private void ThrowIfSealed(string conflictingModifier)
        {
            if (IsSealed)
            {
                ThrowModifierConflict("sealed", conflictingModifier);
            }
        }

        private void ThrowModifierConflict(string modifier, string conflictingModifier)
        {
            throw new InvalidOperationException(
                $"Class '{Name}' cannot be both {modifier} and {conflictingModifier}.");
        }

        Expression IClassExpressionConfigurator.BaseInstanceExpression
            => _baseInstanceExpression ??= new InstanceExpression(_baseTypeExpression, "base");

        void IClassExpressionConfigurator.SetBaseType(
            Type baseType,
            Action<IClassImplementationConfigurator> configuration)
        {
            SetBaseType(baseType, configuration);
        }

        internal void SetBaseType(Type baseType)
            => SetBaseType(baseType, configuration: null);

        private void SetBaseType(
            Type baseType,
            Action<IClassImplementationConfigurator> configuration)
        {
            baseType.ThrowIfNull(nameof(baseType));
            ThrowIfBaseTypeAlreadySet(baseType);
            ThrowIfInvalidBaseType(baseType);

            if (configuration == null)
            {
                SetBaseTypeTo(baseType);
                return;
            }

            var configurator = new ImplementationConfigurator(this, baseType);
            configuration.Invoke(configurator);
            SetBaseTypeTo(configurator.GetImplementedType());
        }

        private void ThrowIfBaseTypeAlreadySet(Type baseType)
        {
            if (BaseType != typeof(object))
            {
                throw new InvalidOperationException(
                    $"Unable to set class base type to '{baseType.GetFriendlyName()}' " +
                    $"as it has already been set to '{BaseType.GetFriendlyName()}'");
            }
        }

        private static void ThrowIfInvalidBaseType(Type baseType)
        {
            if (!baseType.IsClass() || baseType.IsSealed())
            {
                throw new InvalidOperationException(
                    $"Type '{baseType.GetFriendlyName()}' is not a valid base type.");
            }
        }

        private void SetBaseTypeTo(Type baseType)
        {
            BaseType = baseType;

            _baseTypeExpression = (ClassExpression)SourceCode
                .TypeExpressions
                .FirstOrDefault(t => t.TypeAccessor == baseType);
        }

        MethodExpression IClassMethodConfigurator.AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return AddMethod(name, configuration);
        }

        internal override StandardMethodExpression Add(StandardMethodExpression method)
        {
            if (IsStatic)
            {
                ((IConcreteTypeMethodExpressionConfigurator)method).SetStatic();
            }

            return base.Add(method);
        }

        #endregion

        internal override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}