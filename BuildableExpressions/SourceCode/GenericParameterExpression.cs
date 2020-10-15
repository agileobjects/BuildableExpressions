﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using Compilation;
    using Extensions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents an open class or method generic argument.
    /// </summary>
    public class GenericParameterExpression :
        Expression,
        IGenericParameterNamingContext,
        IGenericParameterExpressionConfigurator,
        IGenericArgument,
        ICustomTranslationExpression
    {
        private bool _hasConstraints;
        private bool _hasStructConstraint;
        private bool _hasClassConstraint;
        private bool _hasNewableConstraint;
        private List<Type> _typeConstraints;
        private ReadOnlyCollection<Type> _readonlyTypeConstraints;

        internal GenericParameterExpression(
            string name,
            Func<IGenericParameterExpressionConfigurator, IGenericParameterExpressionConfigurator> configuration)
        {
            Name = name.ThrowIfInvalidName<ArgumentException>("Generic Parameter");

            configuration.Invoke(this);

            Type = CreateType();
        }

        #region Setup

        private Type CreateType()
        {
            var parametersSourceCode = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace(BuildConstants.GenericParameterTypeNamespace)
                    .WithClass(cls =>
                    {
                        if (!_hasClassConstraint)
                        {
                            cls.AsValueType();
                        }

                        return cls.Named(Name);
                    }))
                .ToSourceCode();

            var compiledTypes = Compiler.Instance
                .Compile(new[] { parametersSourceCode })
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        /// <summary>
        /// Gets the type of this <see cref="GenericParameterExpression"/>, which is 'void', as this
        /// class represents an open generic argument.
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// Visits this <see cref="GenericParameterExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="GenericParameterExpression"/>.
        /// </param>
        /// <returns>This <see cref="GenericParameterExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets this <see cref="GenericParameterExpression"/>'s parent <see cref="MethodExpression"/>.
        /// </summary>
        public MethodExpression Method { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="GenericParameterExpression"/>
        /// </summary>
        public string Name { get; }

        #region IGenericParameterNamingContext Members

        int IGenericParameterNamingContext.Index => Method?.GenericArguments.IndexOf(this) ?? 0;

        #endregion

        #region IGenericParameterExpressionConfigurator Members

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithStructConstraint()
        {
            _hasConstraints = _hasStructConstraint = true;
            return this;
        }

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithClassConstraint()
        {
            _hasConstraints = _hasClassConstraint = true;
            return this;
        }

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithNewableConstraint()
        {
            _hasConstraints = _hasNewableConstraint = true;
            return this;
        }

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithTypeConstraint<T>()
            => AddTypeConstraints(typeof(T));

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithTypeConstraint(Type type)
            => AddTypeConstraints(type);

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.WithTypeConstraints(params Type[] types)
            => AddTypeConstraints(types);

        private IGenericParameterExpressionConfigurator AddTypeConstraints(params Type[] types)
        {
            _hasConstraints = true;
            _typeConstraints ??= new List<Type>();
            _typeConstraints.AddRange(types);
            _readonlyTypeConstraints = null;
            return this;
        }

        #endregion

        #region IGenericArgument Members

        /// <inheritdoc />
        public bool IsClosed => false;

        bool IGenericArgument.HasConstraints => _hasConstraints;

        bool IGenericArgument.HasClassConstraint => _hasClassConstraint;

        bool IGenericArgument.HasStructConstraint => _hasStructConstraint;

        bool IGenericArgument.HasNewableConstraint => _hasNewableConstraint;

        ReadOnlyCollection<Type> IGenericArgument.TypeConstraints
        {
            get
            {
                return _readonlyTypeConstraints ??=
                       _typeConstraints?.ToReadOnlyCollection() ??
                       Enumerable<Type>.EmptyReadOnlyCollection;
            }
        }

        #endregion

        internal void SetMethod(MethodExpression owningMethod)
        {
            if (Method == null)
            {
                Method = owningMethod;
                return;
            }

            throw new InvalidOperationException(
                 "Unable to add generic parameter to method " +
                $"'{owningMethod.Class.Name}.{owningMethod.Name}' - " +
                 "this parameter has already been added to method " +
                $"'{Method.Class.Name}.{Method.Name}'");
        }

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => context.GetTranslationFor(Type);
    }
}