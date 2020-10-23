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
        IMethodExpressionConfigurator,
        IMethod,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        private List<ParameterExpression> _parameters;
        private List<GenericParameterExpression> _genericArguments;
        private ReadOnlyCollection<GenericParameterExpression> _readonlyGenericParameters;
        private ReadOnlyCollection<IGenericArgument> _readonlyGenericArguments;
        private ReadOnlyCollection<IParameter> _readonlyParameters;
        private MethodInfo _methodInfo;

        internal MethodExpression(
            TypeExpression declaringTypeExpression,
            string name,
            Action<IMethodExpressionConfigurator> configuration)
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

        /// <summary>
        /// Gets this <see cref="MethodExpression"/>'s parent <see cref="TypeExpression"/>.
        /// </summary>
        public TypeExpression DeclaringTypeExpression { get; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="MethodExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Gets the <see cref="MemberVisibility"/> of this <see cref="MethodExpression"/>.
        /// </summary>
        public MemberVisibility Visibility { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is static.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpression"/> is generic.
        /// </summary>
        public bool IsGeneric => _genericArguments?.Any() == true;

        /// <summary>
        /// Gets the name of this <see cref="MethodExpression"/>.
        /// </summary>
        public string Name { get; internal set; }

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
        public Type ReturnType => Definition?.ReturnType ?? typeof(void);

        /// <summary>
        /// Gets the <see cref="IGenericArgument"/>s describing the generic arguments of this
        /// <see cref="MethodExpression"/>, if any.
        /// </summary>
        public ReadOnlyCollection<GenericParameterExpression> GenericArguments
        {
            get
            {
                return _readonlyGenericParameters ??= IsGeneric
                    ? _genericArguments.ToReadOnlyCollection()
                    : Enumerable<GenericParameterExpression>.EmptyReadOnlyCollection;
            }
        }

        internal IList<GenericParameterExpression> GenericArgumentsAccessor
            => _genericArguments;

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
            => Definition?.Parameters ?? Enumerable<ParameterExpression>.EmptyReadOnlyCollection;

        internal IList<ParameterExpression> ParametersAccessor => _parameters;

        /// <summary>
        /// Gets the Expression describing the body of this <see cref="MethodExpression"/>.
        /// </summary>
        public Expression Body => Definition?.Body;

        #region IMethodExpressionConfigurator Members

        void IMethodExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void IMethodExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => Visibility = visibility;

        void IMethodExpressionConfigurator.SetStatic()
            => IsStatic = true;

        GenericParameterExpression IMethodExpressionConfigurator.AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration)
        {
            var parameter = new GenericParameterExpression(this, name, configuration);

            _genericArguments ??= new List<GenericParameterExpression>();
            _readonlyGenericParameters = null;
            _readonlyGenericArguments = null;

            _genericArguments.Add(parameter);
            return parameter;
        }

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

        void IMethodExpressionConfigurator.SetBody(Expression body, Type returnType)
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

        #region IMethod Members

        Type IMethod.DeclaringType => null;

        bool IMethod.IsPublic => Visibility == Public;

        bool IMethod.IsProtectedInternal => Visibility == ProtectedInternal;

        bool IMethod.IsInternal => Visibility == Internal;

        bool IMethod.IsProtected => Visibility == Protected;

        bool IMethod.IsPrivate => Visibility == Private;

        bool IMethod.IsAbstract => false;

        bool IMethod.IsVirtual => false;

        bool IMethod.IsGenericMethod => IsGeneric;

        bool IMethod.IsExtensionMethod => false;

        IMethod IMethod.GetGenericMethodDefinition() => null;

        ReadOnlyCollection<IGenericArgument> IMethod.GetGenericArguments()
        {
            return _readonlyGenericArguments ??= IsGeneric
                ? _genericArguments.ProjectToArray(arg => (IGenericArgument)arg).ToReadOnlyCollection()
                : Enumerable<IGenericArgument>.EmptyReadOnlyCollection;
        }

        ReadOnlyCollection<IParameter> IMethod.GetParameters()
        {
            return _readonlyParameters ??= Parameters
                .ProjectToArray<ParameterExpression, IParameter>(p => new MethodParameter(p))
                .ToReadOnlyCollection();
        }

        #endregion

        IEnumerable<Expression> ICustomAnalysableExpression.Expressions
        {
            get { yield return Definition; }
        }

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