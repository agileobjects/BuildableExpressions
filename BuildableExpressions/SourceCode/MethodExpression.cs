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
        Expression,
        IClassMethodExpressionConfigurator,
        IMethod,
        ICustomTranslationExpression
    {
        private List<ParameterExpression> _parameters;
        private List<GenericParameterExpression> _genericParameters;
        private ReadOnlyCollection<GenericParameterExpression> _readonlyGenericParameters;
        private ReadOnlyCollection<IGenericArgument> _readonlyGenericArguments;
        private ReadOnlyCollection<IParameter> _readonlyParameters;
        private MethodInfo _methodInfo;

        internal MethodExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Action<MethodExpression> configuration)
        {
            DeclaringTypeExpression = declaringTypeExpression;
            Name = name;
            configuration.Invoke(this);
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1003) indicating the type of this
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
            return this;
        }

        internal MethodExpressionAnalysis Analysis { get; set; }

        internal abstract bool HasGeneratedName { get; }

        internal abstract bool HasBody { get; }

        /// <summary>
        /// Gets this <see cref="MethodExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </summary>
        public TypeExpression DeclaringTypeExpression { get; }

        /// <inheritdoc />
        public Type DeclaringType => DeclaringTypeExpression.Type;

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="MethodExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Gets the <see cref="MemberVisibility"/> of this <see cref="MethodExpression"/>.
        /// </summary>
        public MemberVisibility? Visibility { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is static.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="MethodExpression"/>.
        /// </summary>
        public string Name { get; internal set; }

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
                return _readonlyGenericParameters ??= IsGeneric
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
            var declaringType = DeclaringTypeExpression.Type;

            var parameterTypes =
                _parameters?.ProjectToArray(p => p.Type) ??
                Type.EmptyTypes;

            var method = Visibility == Public
                ? declaringType.GetPublicInstanceMethod(Name, parameterTypes)
                : declaringType.GetNonPublicInstanceMethod(Name, parameterTypes);

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
        public virtual ReadOnlyCollection<ParameterExpression> Parameters
            => Definition?.Parameters ?? Enumerable<ParameterExpression>.EmptyReadOnlyCollection;

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
            _readonlyGenericParameters = null;
            _readonlyGenericArguments = null;

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

        #region IMethodExpressionConfigurator Members

        void IMethodExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void IMethodExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => SetVisibility(visibility);

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="MethodExpression"/>.
        /// </param>
        protected void SetVisibility(MemberVisibility visibility)
            => Visibility = visibility;

        void IMethodExpressionConfigurator.AddParameters(
            params ParameterExpression[] parameters)
        {
            AddParameters(parameters);
        }

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
        }

        #endregion

        #region IConcreteTypeMethodExpressionConfigurator

        void IConcreteTypeMethodExpressionConfigurator.SetStatic()
        {
            ThrowIfAbstract();
            IsStatic = true;
        }

        private void ThrowIfAbstract()
        {
            if (IsAbstract)
            {
                ThrowModifierConflict("abstract", "static");
            }
        }

        void IConcreteTypeMethodExpressionConfigurator.SetBody(Expression body, Type returnType)
        {
            if (body.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)body;
                returnType = lambda.ReturnType;
                AddParameters(lambda.Parameters);
                body = lambda.Body;
            }

            Definition = body.ToLambdaExpression(_parameters, returnType);
        }

        #endregion

        #region IClassMethodExpressionConfigurator

        void IClassMethodExpressionConfigurator.SetAbstract()
        {
            ThrowIfNonAbstractClass();
            ThrowIfStatic();

            IsAbstract = true;
        }

        private void ThrowIfNonAbstractClass()
        {
            if (((ClassExpression)DeclaringTypeExpression).IsAbstract)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Unable to add abstract method '{this.GetSignature(includeTypeName: false)}' " +
                $"to non-abstract declaring type '{DeclaringTypeExpression.Name}'.");
        }

        private void ThrowIfStatic()
        {
            if (IsStatic)
            {
                ThrowModifierConflict("static", "abstract");
            }
        }

        private void ThrowModifierConflict(string modifier, string conflictingModifier)
        {
            throw new InvalidOperationException(
                $"Method '{this.GetSignature()}' cannot be " +
                $"both {modifier} and {conflictingModifier}.");
        }

        #endregion

        #region IMethod Members

        bool IMethod.IsPublic => Visibility == Public;

        bool IMethod.IsProtectedInternal => Visibility == ProtectedInternal;

        bool IMethod.IsInternal => Visibility == Internal;

        bool IMethod.IsProtected => Visibility == Protected;

        bool IMethod.IsPrivate => Visibility == Private;

        bool IMethod.IsVirtual => false;

        bool IMethod.IsOverride => false;

        bool IMethod.IsGenericMethod => IsGeneric;

        bool IMethod.IsExtensionMethod => false;

        IMethod IMethod.GetGenericMethodDefinition() => null;

        ReadOnlyCollection<IGenericArgument> IMethod.GetGenericArguments()
        {
            return _readonlyGenericArguments ??= IsGeneric
                ? _genericParameters.ProjectToArray(arg => (IGenericArgument)arg).ToReadOnlyCollection()
                : Enumerable<IGenericArgument>.EmptyReadOnlyCollection;
        }

        ReadOnlyCollection<IParameter> IMethod.GetParameters()
        {
            if (_readonlyParameters != null)
            {
                return _readonlyParameters;
            }

            var parameters = Definition?.Parameters ?? (IList<ParameterExpression>)_parameters;

            var readonlyParameters = parameters
                .ProjectToArray<ParameterExpression, IParameter>(p => new MethodParameter(p))
                .ToReadOnlyCollection();

            if (Definition != null)
            {
                _readonlyParameters = readonlyParameters;
            }

            return readonlyParameters;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Throws an InvalidOperationException if this <see cref="MethodExpression"/> has the same
        /// signature as another method on the <see cref="DeclaringTypeExpression"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpression"/> has the same signature as another method
        /// on the <see cref="DeclaringTypeExpression"/>.
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

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => new MethodTranslation(this, context);

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