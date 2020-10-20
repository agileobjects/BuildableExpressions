namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a piece of complete source code.
    /// </summary>
    public class SourceCodeExpression :
        Expression,
        ISourceCodeExpressionConfigurator,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        private readonly List<TypeExpression> _typeExpressions;
        private ReadOnlyCollection<TypeExpression> _readOnlyTypeExpressions;
        private ReadOnlyCollection<Assembly> _referencedAssemblies;

        internal SourceCodeExpression(Action<ISourceCodeExpressionConfigurator> configuration)
        {
            _typeExpressions = new List<TypeExpression>();
            Namespace = "GeneratedExpressionCode";

            configuration.Invoke(this);

            Analysis = SourceCodeAnalysis.For(this);
        }

        internal SourceCodeAnalysis Analysis { get; }

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
        /// Gets the namespace to which the source code represented by this
        /// <see cref="SourceCodeExpression"/> belongs.
        /// </summary>
        public string Namespace { get; private set; }

        internal ReadOnlyCollection<Assembly> ReferencedAssemblies
            => _referencedAssemblies ??= GetReferenceAssemblies().ToReadOnlyCollection();

        #region ReferencedAssemblies Helpers

        private IList<Assembly> GetReferenceAssemblies()
        {
            var referenceAssemblies = new List<Assembly>();

            foreach (var type in TypeExpressions.SelectMany(t => t.ImplementedTypes).Distinct())
            {
#if NETFRAMEWORK
                AddAssembliesFor(type, referenceAssemblies);
#else
                AddAssemblyFor(type, referenceAssemblies);
#endif
            }

            return referenceAssemblies;
        }

#if NETFRAMEWORK
        private static void AddAssembliesFor(Type type, ICollection<Assembly> assemblies)
        {
            while (true)
            {
                AddAssemblyFor(type, assemblies);

                // ReSharper disable once PossibleNullReferenceException
                var baseType = type.BaseType;

                while (baseType != null && baseType != typeof(object))
                {
                    AddAssemblyFor(baseType, assemblies);
                    baseType = baseType.BaseType;
                }

                if (!type.IsNested)
                {
                    return;
                }

                type = type.DeclaringType;
            }
        }
#endif
        private static void AddAssemblyFor(Type type, ICollection<Assembly> assemblies)
        {
            var assembly = type.GetAssembly();

            if (!assemblies.Contains(assembly))
            {
                assemblies.Add(assembly);
            }
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="TypeExpression"/>s which describe the types defined by this
        /// <see cref="SourceCodeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<TypeExpression> TypeExpressions
            => _readOnlyTypeExpressions ??= _typeExpressions.ToReadOnlyCollection();

        internal void Finalise(IList<TypeExpression> typeExpressions)
        {
            if (_typeExpressions.SequenceEqual(typeExpressions))
            {
                return;
            }

            _typeExpressions.Clear();
            _typeExpressions.AddRange(typeExpressions);
        }

        /// <summary>
        /// Translates this <see cref="SourceCodeExpression"/> to a complete source-code string,
        /// formatted as one or more types with one or more methods in a namespace.
        /// </summary>
        /// <returns>
        /// The translated <see cref="SourceCodeExpression"/>, formatted as one or more types with
        /// one or more methods in a namespace.
        /// </returns>
        public string ToCSharpString()
        {
            Validate();
            return new SourceCodeExpressionTranslation(this).GetTranslation();
        }

        private void Validate()
        {
            ThrowIfNoClasses();
            ThrowIfDuplicateClassName();

            foreach (var @class in _typeExpressions)
            {
                @class.Validate();
            }
        }

        private void ThrowIfNoClasses()
        {
            if (_typeExpressions.Any())
            {
                return;
            }

            throw new InvalidOperationException("At least one class must be specified");
        }

        private void ThrowIfDuplicateClassName()
        {
            var duplicateClassName = _typeExpressions
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

        void ISourceCodeExpressionConfigurator.SetNamespace(string @namespace)
            => Namespace = @namespace;

        ClassExpression ISourceCodeExpressionConfigurator.AddClass(
            string name,
            Action<IClassExpressionConfigurator> configuration)
        {
            return new ClassExpression(this, name, configuration);
        }

        StructExpression ISourceCodeExpressionConfigurator.AddStruct(
            string name,
            Action<IStructExpressionConfigurator> configuration)
        {
            return new StructExpression(this, name, configuration);
        }

        internal void Register(TypeExpression type)
        {
            _typeExpressions.Add(type);
            _readOnlyTypeExpressions = null;
        }

        #endregion

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
            => TypeExpressions.Cast<ICustomAnalysableExpression>().SelectMany(c => c.Expressions);

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new SourceCodeTranslation(this, context);
    }
}
