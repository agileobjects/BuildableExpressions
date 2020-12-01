namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;

    internal class ConfiguredClassExpression :
        ClassExpression,
        IClassExpressionConfigurator
    {
        private Expression _baseInstanceExpression;

        public ConfiguredClassExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IClassExpressionConfigurator> configuration)
            : this(sourceCode, name)
        {
            configuration.Invoke(this);
            Validate();
        }

        private ConfiguredClassExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IClassExpressionConfigurator Members

        void IClassExpressionConfigurator.SetImplements(
            InterfaceExpression interfaceExpression,
            Action<IClassImplementationConfigurator> configuration)
        {
            SetImplements(interfaceExpression, configuration);
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
            ClassExpression baseTypeExpression,
            Action<IClassImplementationConfigurator> configuration)
        {
            baseTypeExpression.ThrowIfNull(nameof(baseTypeExpression));
            ThrowIfBaseTypeAlreadySet(baseTypeExpression);
            ThrowIfInvalidBaseType(baseTypeExpression);

            if (configuration != null)
            {
                var configurator = new ImplementationConfigurator(
                    this,
                    baseTypeExpression,
                    configuration);

                baseTypeExpression = (ClassExpression)configurator.ImplementedTypeExpression;
            }

            BaseTypeExpression = baseTypeExpression;
        }

        private void ThrowIfBaseTypeAlreadySet(IType baseType)
        {
            if (!HasObjectBaseType)
            {
                throw new InvalidOperationException(
                    $"Unable to set class base type to '{baseType.Name}' " +
                    $"as it has already been set to '{BaseTypeExpression.Name}'");
            }
        }

        private static void ThrowIfInvalidBaseType(ClassExpression baseType)
        {
            if (baseType.IsSealed)
            {
                throw new InvalidOperationException(
                    $"Type '{baseType.Name}' is not a valid base type.");
            }
        }

        PropertyExpression IClassMemberConfigurator.AddProperty(
            string name,
            IType type,
            Action<IClassPropertyExpressionConfigurator> configuration)
        {
            return AddProperty(name, type, configuration);
        }

        MethodExpression IClassMemberConfigurator.AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return AddMethod(name, configuration);
        }

        internal override MethodExpression AddMethod(MethodExpression method)
        {
            if (IsStatic)
            {
                ((IConcreteTypeMethodExpressionConfigurator)method).SetStatic();
            }

            return base.AddMethod(method);
        }

        #endregion

        protected override TypeExpression CreateInstance()
        {
            return new ConfiguredClassExpression(SourceCode, Name)
            {
                BaseTypeExpression = BaseTypeExpression,
                IsStatic = IsStatic,
                IsAbstract = IsAbstract,
                IsSealed = IsSealed
            };
        }

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}