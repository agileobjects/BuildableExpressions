namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;
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

            if (!IsAbstract)
            {
                return;
            }

            var parameterlessCtorExpression = ConstructorExpressionsAccessor?
                .FirstOrDefault(ctor => ctor.ParametersAccessor == null);

            if (parameterlessCtorExpression == null)
            {
                AddDefaultConstructor();
            }
        }

        private ConfiguredClassExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        #region IClosableTypeExpression Members

        protected override ClassExpression CreateInstance()
        {
            return new ConfiguredClassExpression(SourceCode, Name)
            {
                BaseTypeExpression = BaseTypeExpression,
                IsStatic = IsStatic,
                IsAbstract = IsAbstract,
                IsSealed = IsSealed
            };
        }

        #endregion

        #region IClassBaseExpressionConfigurator Members

        void IClassBaseExpressionConfigurator.SetAbstract() => SetAbstract();

        internal void SetAbstract()
        {
            ThrowIfStatic("abstract");
            ThrowIfSealed("abstract");
            IsAbstract = true;
        }

        void IClassBaseExpressionConfigurator.SetSealed()
        {
            ThrowIfStatic("sealed");
            ThrowIfAbstract("sealed");
            IsSealed = true;
        }

        Expression IClassBaseExpressionConfigurator.BaseInstanceExpression
            => _baseInstanceExpression ??= InstanceExpression.Base(BaseTypeExpression);

        #endregion

        #region IClassMemberConfigurator Members

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

        private void ThrowIfStatic(string conflictingModifier)
        {
            if (IsStatic)
            {
                ThrowModifierConflict("static", conflictingModifier);
            }
        }

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
                ThrowBaseTypeAlreadySet(baseType, typeName: "class");
            }
        }

        #endregion

        protected override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}