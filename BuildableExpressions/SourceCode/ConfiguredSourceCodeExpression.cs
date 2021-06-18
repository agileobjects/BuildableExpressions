namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Analysis;
    using Api;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using Translations;

    internal class ConfiguredSourceCodeExpression :
        SourceCodeExpression,
        ISourceCodeExpressionConfigurator,
        ICustomTranslationExpression
    {
        private readonly List<TypeExpression> _typeExpressions;
        private ReadOnlyCollection<TypeExpression> _readOnlyTypeExpressions;
        private ReadOnlyCollection<Assembly> _referencedAssemblies;

        public ConfiguredSourceCodeExpression(
            Action<ISourceCodeExpressionConfigurator> configuration)
            : this(SourceCodeConstants.DefaultGeneratedCodeNamespace)
        {
            configuration.Invoke(this);
            Validate();

            IsComplete = true;
            Analyse();
        }

        #region Validation

        private void Validate()
        {
            ThrowIfNoTypes();
        }

        private void ThrowIfNoTypes()
        {
            if (!TypeExpressions.Any())
            {
                throw new InvalidOperationException("At least one type must be specified");
            }
        }

        #endregion

        public ConfiguredSourceCodeExpression(ConfiguredSourceCodeExpression parent)
            : this(parent.Namespace)
        {
            Analysis = parent.Analysis;
        }

        public ConfiguredSourceCodeExpression(string @namespace)
            : base(@namespace)
        {
            _typeExpressions = new List<TypeExpression>();
        }

        internal override bool IsComplete { get; }

        internal SourceCodeAnalysis Analysis { get; private set; }

        public override ReadOnlyCollection<Assembly> ReferencedAssemblies
            => _referencedAssemblies ??= Analysis.RequiredAssemblies.ToReadOnlyCollection();

        public override ReadOnlyCollection<TypeExpression> TypeExpressions
            => _readOnlyTypeExpressions ??= _typeExpressions.ToReadOnlyCollection();

        public override string TypeName => _typeExpressions.First().Name;

        #region ISourceCodeExpressionConfigurator Members

        void ISourceCodeExpressionConfigurator.SetHeader(CommentExpression header)
            => Header = header;

        void ISourceCodeExpressionConfigurator.SetNamespace(string @namespace)
            => Namespace = @namespace;

        InterfaceExpression ISourceCodeExpressionConfigurator.AddInterface(
            string name,
            Action<IInterfaceExpressionConfigurator> configuration)
        {
            return Add(new ConfiguredInterfaceExpression(this, name, configuration));
        }

        ClassExpression ISourceCodeExpressionConfigurator.AddClass(
            string name,
            Action<IClassExpressionConfigurator> configuration)
        {
            return Add(new ConfiguredClassExpression(this, name, configuration));
        }

        StructExpression ISourceCodeExpressionConfigurator.AddStruct(
            string name,
            Action<IStructExpressionConfigurator> configuration)
        {
            return Add(new ConfiguredStructExpression(this, name, configuration));
        }

        AttributeExpression ISourceCodeExpressionConfigurator.AddAttribute(
            string name,
            Action<IAttributeExpressionConfigurator> configuration)
        {
            return Add(new ConfiguredAttributeExpression(this, name, configuration));
        }

        EnumExpression ISourceCodeExpressionConfigurator.AddEnum(
            string name,
            Action<IEnumExpressionConfigurator> configuration)
        {
            return Add(new ConfiguredEnumExpression(this, name, configuration));
        }

        internal TType Add<TType>(TType type)
            where TType : TypeExpression
        {
            _typeExpressions.Add(type);
            _readOnlyTypeExpressions = null;
            return type;
        }

        #endregion

        internal void Analyse()
        {
            _referencedAssemblies = null;
            Analysis = SourceCodeAnalysis.For(this);
        }

        public override string ToSourceCodeString()
            => new SourceCodeExpressionTranslation(this).GetTranslation();

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new SourceCodeTranslation(this, context);
    }
}