namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;

    internal class ConfiguredClassExpression :
        ClassExpression,
        IClassExpressionConfigurator
    {
        private Expression _baseInstanceExpression;

        internal ConfiguredClassExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IClassExpressionConfigurator> configuration)
            : base(sourceCode, typeof(object), name)
        {
            configuration.Invoke(this);
            Validate();
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
            => _baseInstanceExpression ??= InstanceExpression.Base(BaseTypeExpression);

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
            if (!HasObjectBaseType)
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
    }
}