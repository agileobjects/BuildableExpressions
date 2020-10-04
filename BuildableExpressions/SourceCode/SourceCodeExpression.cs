namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using Extensions;
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
        private readonly SourceCodeTranslationSettings _settings;
        private readonly List<ClassExpression> _classes;
        private ReadOnlyCollection<ClassExpression> _readOnlyClasses;

        internal SourceCodeExpression(SourceCodeTranslationSettings settings)
        {
            _settings = settings;
            _classes = new List<ClassExpression>();
            Namespace = "GeneratedExpressionCode";
        }

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
        /// Visits each of this <see cref="SourceCodeExpression"/>'s <see cref="Classes"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="SourceCodeExpression"/>'s
        /// <see cref="Classes"/>.
        /// </param>
        /// <returns>This <see cref="SourceCodeExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            foreach (var @class in Classes)
            {
                visitor.Visit(@class);
            }

            return this;
        }

        /// <summary>
        /// Gets the namespace to which the source code represented by this
        /// <see cref="SourceCodeExpression"/> belongs.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClassExpression"/>s which describe the classes of this
        /// <see cref="SourceCodeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<ClassExpression> Classes
            => _readOnlyClasses ??= _classes.ToReadOnlyCollection();

        /// <summary>
        /// Adds a new <see cref="ClassExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <returns>The newly-created <see cref="ClassExpression"/>.</returns>
        public ClassExpression AddClass() => AddClass(cfg => cfg);

        /// <summary>
        /// Adds a new <see cref="ClassExpression"/> to this <see cref="SourceCodeExpression"/>.
        /// </summary>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="ClassExpression"/>
        /// .</param>
        /// <returns>The newly-created <see cref="ClassExpression"/>.</returns>
        public ClassExpression AddClass(
            Func<IClassExpressionConfigurator, IClassExpressionConfigurator> configuration)
        {
            var @class = new ClassExpression(this, _settings);
            configuration.Invoke(@class);

            _classes.Add(@class);
            _readOnlyClasses = null;
            return @class;
        }

        internal void Finalise(IList<ClassExpression> classes)
        {
            if (_classes.SequenceEqual(classes))
            {
                return;
            }

            _classes.Clear();
            _classes.AddRange(classes);
        }

        /// <summary>
        /// Translates this <see cref="SourceCodeExpression"/> to a complete source-code string,
        /// formatted as one or more classes with one or more methods in a namespace.
        /// </summary>
        /// <returns>
        /// The translated <see cref="SourceCodeExpression"/>, formatted as one or more classes with
        /// one or more methods in a namespace.
        /// </returns>
        public string ToSourceCode()
        {
            Validate();
            return new SourceCodeExpressionTranslation(this, _settings).GetTranslation();
        }

        private void Validate()
        {
            ThrowIfNoClasses();
            ThrowIfDuplicateClassName();

            foreach (var @class in _classes)
            {
                @class.Validate();
            }
        }

        private void ThrowIfNoClasses()
        {
            if (_classes.Any())
            {
                return;
            }

            throw new InvalidOperationException("At least one class must be specified");
        }

        private void ThrowIfDuplicateClassName()
        {
            var duplicateClassName = _classes
                .GroupBy(cls => cls.Name)
                .FirstOrDefault(nameGroup => nameGroup.Count() > 1)?
                .Key;

            if (duplicateClassName != null)
            {
                throw new InvalidOperationException(
                    $"Duplicate class name '{duplicateClassName}' specified.");
            }
        }

        #region ISourceCodeExpressionConfigurator Members

        ISourceCodeExpressionConfigurator ISourceCodeExpressionConfigurator.WithNamespaceOf<T>()
        {
            Namespace = typeof(T).Namespace;
            return this;
        }

        ISourceCodeExpressionConfigurator ISourceCodeExpressionConfigurator.WithNamespaceOf(Type type)
        {
            Namespace = type.Namespace;
            return this;
        }

        ISourceCodeExpressionConfigurator ISourceCodeExpressionConfigurator.WithNamespace(
            string @namespace)
        {
            Namespace = @namespace;
            return this;
        }

        ISourceCodeExpressionConfigurator ISourceCodeExpressionConfigurator.WithClass(
            Func<IClassExpressionConfigurator, IClassExpressionConfigurator> configuration)
        {
            AddClass(configuration);
            return this;
        }

        #endregion

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            if (!(context is ISourceCodeTranslationContext sourceCodeContext))
            {
                sourceCodeContext = new CompositeSourceCodeTranslationContext(
                    NamespaceAnalysis.For(this, _settings), 
                    context);
            }

            return new SourceCodeTranslation(this, sourceCodeContext);
        }
    }
}
