namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Analysis;
    using Api;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a piece of complete source code.
    /// </summary>
    public class SourceCodeExpression :
        Expression,
        ISourceCodeExpressionConfigurator,
        ICustomTranslationExpression
    {
        private readonly List<TypeExpression> _typeExpressions;
        private ReadOnlyCollection<TypeExpression> _readOnlyTypeExpressions;
        private ReadOnlyCollection<Assembly> _referencedAssemblies;

        internal SourceCodeExpression(Action<ISourceCodeExpressionConfigurator> configuration)
            : this("GeneratedExpressionCode")
        {
            configuration.Invoke(this);
            Validate();

            IsComplete = true;
            Analyse();
        }

        internal SourceCodeExpression(SourceCodeExpression parent)
            : this(parent.Namespace)
        {
            Analysis = parent.Analysis;
        }

        internal SourceCodeExpression(string @namespace)
        {
            _typeExpressions = new List<TypeExpression>();
            Namespace = @namespace;
        }

        #region Validation

        private void Validate()
        {
            ThrowIfNoTypes();
        }

        private void ThrowIfNoTypes()
        {
            if (_typeExpressions.Any())
            {
                return;
            }

            throw new InvalidOperationException("At least one type must be specified");
        }

        #endregion

        internal bool IsComplete { get; }

        internal SourceCodeAnalysis Analysis { get; private set; }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1000) indicating the type of this
        /// <see cref="SourceCodeExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.SourceCode;

        /// <summary>
        /// Gets the type of this <see cref="SourceCodeExpression"/> - typeof(void).
        /// </summary>
        public override Type Type => typeof(void);

        /// <summary>
        /// Visits each of this <see cref="SourceCodeExpression"/>'s <see cref="TypeExpressions"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="SourceCodeExpression"/>'s
        /// <see cref="TypeExpressions"/>.
        /// </param>
        /// <returns>This <see cref="SourceCodeExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            foreach (var @class in TypeExpressions)
            {
                visitor.Visit(@class);
            }

            return this;
        }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> containing the file header to use in the source
        /// code generated from this <see cref="SourceCodeExpression"/>, if a header has been set.
        /// </summary>
        public CommentExpression Header { get; private set; }

        /// <summary>
        /// Gets the namespace to which the source code represented by this
        /// <see cref="SourceCodeExpression"/> belongs.
        /// </summary>
        public string Namespace { get; private set; }

        internal ReadOnlyCollection<Assembly> ReferencedAssemblies
            => _referencedAssemblies ??= Analysis.RequiredAssemblies.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="TypeExpression"/>s which describe the types defined by this
        /// <see cref="SourceCodeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<TypeExpression> TypeExpressions
            => _readOnlyTypeExpressions ??= _typeExpressions.ToReadOnlyCollection();

        /// <summary>
        /// Translates this <see cref="SourceCodeExpression"/> to a complete C# source-code string,
        /// formatted as one or more types with one or more methods in a namespace.
        /// </summary>
        /// <returns>
        /// The translated <see cref="SourceCodeExpression"/>, formatted as one or more types with
        /// one or more methods in a namespace.
        /// </returns>
        public string ToCSharpString()
            => new SourceCodeExpressionTranslation(this).GetTranslation();

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

        internal void Analyse()
        {
            _referencedAssemblies = null;
            Analysis = SourceCodeAnalysis.For(this);
        }

        #endregion

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new SourceCodeTranslation(this, context);
    }
}
