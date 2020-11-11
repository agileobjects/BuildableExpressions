﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Analysis;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Represents a method in a type in a piece of source code.
    /// </summary>
    public abstract class MethodExpression :
        MemberExpression,
        IConcreteTypeMethodExpressionConfigurator,
        IMethod,
        IHasSignature,
        IConcreteTypeExpression
    {
        private List<ParameterExpression> _parameters;
        private List<GenericParameterExpression> _genericParameters;
        private ReadOnlyCollection<GenericParameterExpression> _readOnlyGenericParameters;
        private ReadOnlyCollection<IGenericArgument> _readOnlyGenericArguments;
        private ReadOnlyCollection<IParameter> _readOnlyParameters;
        private MethodInfo _methodInfo;
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExpression"/> class.
        /// </summary>
        /// <param name="declaringTypeExpression">
        /// This <see cref="MethodExpression"/>'s parent <see cref="TypeExpression"/>.</param>
        /// <param name="name">The name of this <see cref="MethodExpression"/>.</param>
        protected MethodExpression(TypeExpression declaringTypeExpression, string name)
            : base(declaringTypeExpression, name)
        {
            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1009) indicating the type of this
        /// <see cref="MethodExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Method;

        /// <summary>
        /// Gets the type of this <see cref="MethodExpression"/>, which is the return type of the
        /// LambdaExpression from which the method was created.
        /// </summary>
        public override Type Type => ReturnType;

        /// <summary>
        /// Visits this <see cref="MethodExpression"/>'s Body.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="MethodExpression"/>.</param>
        /// <returns>This <see cref="MethodExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(Summary);

            foreach (var parameter in Parameters)
            {
                visitor.Visit(parameter);
            }

            visitor.Visit(Body);

            return base.Accept(visitor);
        }

        internal MethodExpressionAnalysis Analysis { get; private protected set; }

        internal abstract bool HasGeneratedName { get; }

        internal abstract bool HasBody { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is virtual.
        /// </summary>
        public bool IsVirtual { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> overrides a method
        /// from a base type.
        /// </summary>
        public abstract bool IsOverride { get; }

        /// <inheritdoc cref="MemberExpression" />
        public override string Name => _name;

        internal void SetName(string name) => _name = name;

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is generic.
        /// </summary>
        public bool IsGeneric => _genericParameters?.Any() == true;

        /// <summary>
        /// Gets the <see cref="GenericParameterExpression"/>s describing the open generic arguments
        /// of this <see cref="MethodExpression"/>, if any.
        /// </summary>
        public ReadOnlyCollection<GenericParameterExpression> GenericParameters
        {
            get
            {
                return _readOnlyGenericParameters ??= IsGeneric
                    ? _genericParameters.ToReadOnlyCollection()
                    : Enumerable<GenericParameterExpression>.EmptyReadOnlyCollection;
            }
        }

        internal IList<GenericParameterExpression> GenericParametersAccessor
            => _genericParameters;

        /// <summary>
        /// Gets the MethodInfo for this <see cref="MethodExpression"/>, which is lazily, dynamically
        /// generated using this method's definition.
        /// </summary>
        public MethodInfo MethodInfo
            => _methodInfo ??= CreateMethodInfo();

        #region MethodInfo Creation

        private MethodInfo CreateMethodInfo()
        {
            var parameterTypes =
                _parameters?.ProjectToArray(p => p.Type) ??
                Type.EmptyTypes;

            var method = Visibility == Public
                ? IsStatic
                    ? DeclaringType.GetPublicStaticMethod(Name, parameterTypes)
                    : DeclaringType.GetPublicInstanceMethod(Name, parameterTypes)
                : IsStatic
                    ? DeclaringType.GetNonPublicStaticMethod(Name, parameterTypes)
                    : DeclaringType.GetNonPublicInstanceMethod(Name, parameterTypes);

            return method;
        }

        #endregion

        /// <summary>
        /// Gets the return type of this <see cref="MethodExpression"/>, which is the return type
        /// of the LambdaExpression from which the method was created.
        /// </summary>
        public virtual Type ReturnType => Definition?.ReturnType ?? typeof(void);

        /// <summary>
        /// Gets the LambdaExpression describing the parameters and body of this
        /// <see cref="MethodExpression"/>.
        /// </summary>
        public LambdaExpression Definition { get; private set; }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/>s describing the parameters of this
        /// <see cref="MethodExpression"/>, if any.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Parameters
            => Definition?.Parameters ?? _parameters.ToReadOnlyCollection();

        internal IList<ParameterExpression> ParametersAccessor => _parameters;

        /// <summary>
        /// Gets the Expression describing the body of this <see cref="MethodExpression"/>.
        /// </summary>
        public Expression Body => Definition?.Body;

        #region IGenericParameterConfigurator Members

        GenericParameterExpression IGenericParameterConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            ThrowIfDuplicateGenericParameterName(name);
            ThrowIfTypeGenericParameterNameClash(name);

            var parameter = new ConfiguredOpenGenericArgumentExpression(name, configuration);

            _genericParameters ??= new List<GenericParameterExpression>();
            _readOnlyGenericParameters = null;
            _readOnlyGenericArguments = null;

            _genericParameters.Add(parameter);
            return parameter;
        }

        private void ThrowIfDuplicateGenericParameterName(string name)
        {
            if (!IsGeneric)
            {
                return;
            }

            var hasDuplicateParameterName = GenericParametersAccessor
                .Any(gp => gp.Name == name);

            if (hasDuplicateParameterName)
            {
                throw new InvalidOperationException(
                    $"Method '{DeclaringTypeExpression.Name}.{Name}': " +
                    $"duplicate generic parameter name '{name}' specified.");
            }
        }

        private void ThrowIfTypeGenericParameterNameClash(string genericParameterName)
        {
            if (!DeclaringTypeExpression.IsGeneric)
            {
                return;
            }

            var hasClashingGenericParameter = DeclaringTypeExpression
                .GenericParameters
                .Any(gp => gp.Name == genericParameterName);

            if (hasClashingGenericParameter)
            {
                throw new InvalidOperationException(
                    $"Generic parameter '{genericParameterName}' has the same name as a " +
                    $"generic parameter in declaring type '{DeclaringTypeExpression.Name}'.");
            }
        }

        #endregion

        #region IMemberExpressionConfigurator Members

        void IMemberExpressionConfigurator.SetSummary(CommentExpression summary)
            => SetSummary(summary);

        void IMemberExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        #endregion

        #region IMethodExpressionConfigurator Members

        void IMethodExpressionConfigurator.AddParameters(params ParameterExpression[] parameters)
            => AddParameters(parameters);

        private void AddParameters(IList<ParameterExpression> parameters)
        {
            if (!parameters.Any())
            {
                return;
            }

            if (_parameters == null)
            {
                _parameters = new List<ParameterExpression>(parameters);
                return;
            }

            _parameters.AddRange(parameters.Except(_parameters));
            _readOnlyParameters = null;
        }

        #endregion

        #region IConcreteTypeMethodExpressionConfigurator Members

        void IConcreteTypeMethodExpressionConfigurator.SetStatic()
        {
            this.ValidateSetStatic();
            SetStatic();
        }

        void IConcreteTypeMethodExpressionConfigurator.SetBody(Expression body, Type returnType)
        {
            if (body.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)body;
                AddParameters(lambda.Parameters);

                if (lambda.ReturnType == returnType)
                {
                    Definition = lambda;
                    return;
                }

                body = Convert(lambda.Body, returnType);
            }

            Definition = body.ToLambdaExpression(_parameters, returnType);
        }

        #endregion

        /// <summary>
        /// Mark this <see cref="MethodExpression"/> as abstract.
        /// </summary>
        protected void SetAbstract()
        {
            IsAbstract = true;
            SetVirtual();
        }

        /// <summary>
        /// Mark this <see cref="MethodExpression"/> as virtual.
        /// </summary>
        protected void SetVirtual() => IsVirtual = true;

        string IHasSignature.GetSignature() => this.GetSignature(includeTypeName: false);

        #region IMethod Members

        bool IComplexMember.IsOverride => IsOverride;

        bool IMethod.IsGenericMethod => IsGeneric;

        bool IMethod.IsExtensionMethod => false;

        IMethod IMethod.GetGenericMethodDefinition() => null;

        ReadOnlyCollection<IGenericArgument> IMethod.GetGenericArguments()
        {
            return _readOnlyGenericArguments ??= IsGeneric
                ? _genericParameters.ProjectToArray(arg => (IGenericArgument)arg).ToReadOnlyCollection()
                : Enumerable<IGenericArgument>.EmptyReadOnlyCollection;
        }

        ReadOnlyCollection<IParameter> IMethod.GetParameters()
        {
            if (_readOnlyParameters != null)
            {
                return _readOnlyParameters;
            }

            var parameters = Definition?.Parameters ?? (IList<ParameterExpression>)_parameters;

            var readonlyParameters = parameters
                .ProjectToArray<ParameterExpression, IParameter>(p => new MethodParameter(p))
                .ToReadOnlyCollection();

            if (Definition != null)
            {
                _readOnlyParameters = readonlyParameters;
            }

            return readonlyParameters;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Throws an InvalidOperationException if this <see cref="MethodExpression"/> has the same
        /// signature as another method on the
        /// <see cref="MemberExpression.DeclaringTypeExpression"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpression"/> has the same signature as another method
        /// on the <see cref="MemberExpression.DeclaringTypeExpression"/>.
        /// </exception>
        protected void ThrowIfDuplicateMethodSignature()
        {
            var hasDuplicateMethod = DeclaringTypeExpression
                .MethodExpressions
                .Any(m => m.Name == Name && HasSameParameterTypes(m));

            if (hasDuplicateMethod)
            {
                throw new InvalidOperationException(
                    $"Type {DeclaringTypeExpression.Name} has duplicate " +
                    $"method signature '{this.GetSignature(includeTypeName: false)}'");
            }
        }

        private bool HasSameParameterTypes(MethodExpression otherMethod)
        {
            if (ParametersAccessor == null)
            {
                return otherMethod.ParametersAccessor == null;
            }

            if (otherMethod.ParametersAccessor == null)
            {
                return false;
            }

            var parameterTypes =
                ParametersAccessor.ProjectToArray(p => p.Type);

            return otherMethod.ParametersAccessor
                .Project(p => p.Type)
                .SequenceEqual(parameterTypes);
        }

        #endregion

        #region Translation

        /// <inheritdoc />
        protected override ITranslation GetFullTranslation(ITranslationContext context)
            => new MethodTranslation(this, context);

        /// <inheritdoc />
        protected override ITranslation GetTransientTranslation(ITranslationContext context)
            => new TransientMethodTranslation(this, context);

        #endregion

        internal void Update(Expression updatedBody)
        {
            if (Body != updatedBody)
            {
                Definition = updatedBody.ToLambdaExpression(_parameters, ReturnType);
            }
        }

        #region Helper Class

        private class MethodParameter : IParameter
        {
            public MethodParameter(ParameterExpression parameter)
            {
                Type = parameter.Type;
                Name = parameter.Name;
            }

            public Type Type { get; }

            public string Name { get; }

            public bool IsOut => false;

            public bool IsParamsArray => false;
        }

        #endregion
    }
}