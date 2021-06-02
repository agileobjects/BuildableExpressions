namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using Generics;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Base class representing a constructor or method in a type in a piece of source code.
    /// </summary>
    public abstract class MethodExpressionBase :
        MemberExpression,
        IMethodExpressionBaseConfigurator,
        IMethod,
        IConcreteTypeExpression,
        IHasSignature
    {
        private List<ParameterExpression> _parameters;
        private ReadOnlyCollection<IParameter> _readOnlyParameters;
        private Dictionary<ParameterExpression, MethodParameterInfo> _parameterInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExpressionBase"/> class.
        /// </summary>
        /// <param name="declaringTypeExpression">
        /// This <see cref="MethodExpressionBase"/>'s parent <see cref="TypeExpression"/>.</param>
        /// <param name="name">The name of this <see cref="MethodExpressionBase"/>.</param>
        protected MethodExpressionBase(TypeExpression declaringTypeExpression, string name)
            : base(declaringTypeExpression, name)
        {
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> indicating the type of this
        /// <see cref="MethodExpressionBase"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType => (ExpressionType)SourceCodeNodeType;

        /// <summary>
        /// Gets the type of this <see cref="MethodExpressionBase"/>, which is the return type of
        /// the LambdaExpression from which the method was created.
        /// </summary>
        public override Type Type => ReturnType;

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> indicating the type of this
        /// <see cref="MethodExpressionBase"/>.
        /// </summary>
        protected abstract SourceCodeExpressionType SourceCodeNodeType { get; }

        /// <summary>
        /// Visits this <see cref="MethodExpressionBase"/>'s Summary, <see cref="Parameters"/> and
        /// <see cref="Body"/>.
        /// </summary>
        /// <param name="visitor">The visitor with which to visit this <see cref="MethodExpressionBase"/>.</param>
        /// <returns>This <see cref="MethodExpressionBase"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            base.Accept(visitor);

            foreach (var parameter in Parameters)
            {
                visitor.Visit(parameter);
            }

            visitor.Visit(Body);
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpressionBase"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpressionBase"/> is virtual.
        /// </summary>
        public bool IsVirtual { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpressionBase"/> overrides a
        /// method from a base type.
        /// </summary>
        public abstract bool IsOverride { get; }

        /// <summary>
        /// Gets the LambdaExpression describing the parameters and body of this
        /// <see cref="MethodExpressionBase"/>.
        /// </summary>
        public virtual LambdaExpression Definition { get; private set; }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/>s describing the parameters of this
        /// <see cref="MethodExpressionBase"/>, if any.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Parameters
            => Definition?.Parameters ?? _parameters.ToReadOnlyCollection();

        internal IList<ParameterExpression> ParametersAccessor => _parameters;

        /// <summary>
        /// Gets a value indicating whether this <see cref="MethodExpressionBase"/> is generic.
        /// </summary>
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// Gets the <see cref="GenericParameterExpression"/>s describing the open generic arguments
        /// of this <see cref="MethodExpressionBase"/>, if any.
        /// </summary>
        public abstract ReadOnlyCollection<GenericParameterExpression> GenericParameters { get; }

        internal abstract IList<GenericParameterExpression> GenericParametersAccessor { get; }

        internal virtual bool HasBody => !IsAbstract && Body != Empty();

        /// <summary>
        /// Gets the Expression describing the body of this <see cref="MethodExpressionBase"/>.
        /// </summary>
        public Expression Body => Definition?.Body;

        /// <summary>
        /// Gets the return Type of this <see cref="MethodExpression"/>, which is the return type
        /// of the LambdaExpression from which the method was created.
        /// </summary>
        public virtual Type ReturnType => Definition?.ReturnType ?? typeof(void);

        #region IMethodBase Members

        bool IMethodBase.IsExtensionMethod => false;

        ReadOnlyCollection<IParameter> IMethodBase.GetParameters()
        {
            if (_readOnlyParameters != null)
            {
                return _readOnlyParameters;
            }

            var parameters = Definition?.Parameters ?? (IList<ParameterExpression>)_parameters;

            ReadOnlyCollection<IParameter> readOnlyParameters;

            if (parameters != null)
            {
                var parameterCount = parameters.Count;
                var parameterAbstractions = new IParameter[parameterCount];

                for (var i = 0; i < parameterCount; ++i)
                {
                    var parameter = parameters[i];

                    parameterAbstractions[i] = TryGetParameterInfo(parameter, out var info)
                        ? info : new MethodParameterInfo(parameter);
                }

                readOnlyParameters = parameterAbstractions.ToReadOnlyCollection();
            }
            else
            {
                readOnlyParameters = Enumerable<IParameter>.EmptyReadOnlyCollection;
            }

            if (Definition != null)
            {
                _readOnlyParameters = readOnlyParameters;
            }

            return readOnlyParameters;
        }

        private bool TryGetParameterInfo(
            ParameterExpression parameter,
            out MethodParameterInfo info)
        {
            if (_parameterInfos != null &&
                _parameterInfos.TryGetValue(parameter, out info))
            {
                return true;
            }

            info = null;
            return false;
        }

        #endregion

        #region IMethod Members

        IMethod IMethod.GetGenericMethodDefinition() => null;

        ReadOnlyCollection<IGenericParameter> IMethod.GetGenericArguments() => GetGenericArguments();

        /// <summary>
        /// Gets this <see cref="MethodExpressionBase"/>'s generic arguments, if any.
        /// </summary>
        /// <returns></returns>
        protected abstract ReadOnlyCollection<IGenericParameter> GetGenericArguments();

        bool IMethod.IsGenericMethod => IsGeneric;

        bool IMethod.HasBody => HasBody;

        IType IMethod.ReturnType => GetReturnType();

        /// <summary>
        /// Gets this <see cref="MethodExpressionBase"/>'s return <see cref="IType"/>.
        /// </summary>
        /// <returns>This <see cref="MethodExpressionBase"/>'s return <see cref="IType"/>.</returns>
        protected abstract IType GetReturnType();

        #endregion

        /// <summary>
        /// Mark this <see cref="MethodExpressionBase"/> as abstract.
        /// </summary>
        protected void SetAbstract()
        {
            IsAbstract = true;
            SetVirtual();
        }

        /// <summary>
        /// Mark this <see cref="MethodExpressionBase"/> as virtual.
        /// </summary>
        protected void SetVirtual() => IsVirtual = true;

        /// <summary>
        /// Add the given <paramref name="parameters"/> to this <see cref="MethodExpressionBase"/>.
        /// </summary>
        /// <param name="parameters">The ParameterInfos to add.</param>
        protected void AddParameters(IList<ParameterInfo> parameters)
            => AddParameters(parameters.ProjectToArray(p => Parameter(p.ParameterType, p.Name)));

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to this <see cref="MethodExpressionBase"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpressions to add.</param>
        /// <returns>
        /// True if any of the given <paramref name="parameters"/> did not already exist and were
        /// added, otherwise false.
        /// </returns>
        private bool AddParameters(IList<ParameterExpression> parameters)
        {
            if (!parameters.Any())
            {
                return false;
            }

            if (_parameters == null)
            {
                _parameters = new List<ParameterExpression>(parameters);
                _readOnlyParameters = null;
                return true;
            }

            var lastParameter = _parameters.Last();

            if (TryGetParameterInfo(lastParameter, out var info) && info.IsParamsArray)
            {
                throw new InvalidOperationException(
                    $"New parameters cannot be added after params array '{lastParameter.Name}'.");
            }

            var currentCount = _parameters.Count;
            _parameters.AddRange(parameters.Except(_parameters));

            if (currentCount == _parameters.Count)
            {
                return false;
            }

            _readOnlyParameters = null;
            return true;

        }

        /// <summary>
        /// Sets the body of this <see cref="MethodExpressionBase"/>, using the given
        /// <paramref name="returnType"/>.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        /// <param name="returnType">The return type to use for the method.</param>
        protected void SetBody(Expression body, Type returnType)
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

        /// <summary>
        /// Updates the body of this <see cref="MethodExpressionBase"/>, creating a new Definition
        /// LambdaExpression from the given <paramref name="body"/> using the current method
        /// Parameters. This method updates this <see cref="MethodExpressionBase"/>, it does not
        /// create a new instance.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        public void Update(Expression body)
        {
            if (Body != body)
            {
                Definition = body.ToLambdaExpression(_parameters, Type);
            }
        }

        string IHasSignature.GetSignature() => GetSignature(includeTypeName: false);

        /// <summary>
        /// When overridden in a derived class, gets the signature of this
        /// <see cref="MethodExpressionBase"/>, optionally including the name of the Type to which
        /// it belongs.
        /// </summary>
        /// <param name="includeTypeName">
        /// Whether to include the name of the Type to which this <see cref="MethodExpressionBase"/>
        /// belongs in the signature.
        /// </param>
        /// <returns>The signature of this <see cref="MethodExpressionBase"/></returns>
        protected abstract string GetSignature(bool includeTypeName);

        internal bool TryGetAttributes(
            ParameterExpression parameterExpression,
            out IList<AppliedAttribute> attributes)
        {
            if (TryGetParameterInfo(parameterExpression, out var info))
            {
                attributes = info.AttributesAccessor;
                return attributes != null;
            }

            attributes = null;
            return false;
        }

        #region IMethodExpressionBaseConfigurator Members

        void IMethodExpressionBaseConfigurator.AddParameter(
            ParameterExpression parameter,
            Action<IParameterExpressionConfigurator> configuration)
        {
            parameter.ThrowIfNull(nameof(parameter));

            if (!AddParameters(new[] { parameter }))
            {
                return;
            }

            if (configuration == null)
            {
                return;
            }

            _parameterInfos ??= new Dictionary<ParameterExpression, MethodParameterInfo>();

            var info = new MethodParameterInfo(parameter);
            configuration.Invoke(info);

            _parameterInfos[parameter] = info;
        }

        void IMethodExpressionBaseConfigurator.AddParameters(params ParameterExpression[] parameters)
            => AddParameters(parameters);

        #endregion

        #region Validation

        /// <summary>
        /// Validates that this <see cref="MethodExpressionBase"/> is non-empty and has a unique
        /// signature.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpressionBase"/> is non-abstract and has no body.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpressionBase"/> has the same signature as another
        /// method on its <see cref="MemberExpression.DeclaringTypeExpression"/>.
        /// </exception>
        protected void Validate()
        {
            ThrowIfEmpty();
            ThrowIfDuplicateSignature();
        }

        private void ThrowIfEmpty()
        {
            if (IsAbstract || Body != null)
            {
                return;
            }

            var methodTypeName = MethodTypeName.ToLowerInvariant();

            throw new InvalidOperationException(
                $"Method '{GetSignature(includeTypeName: true)}': no {methodTypeName} body defined. " +
                $"To add an empty {methodTypeName}, use SetBody(Expression.Empty())");
        }

        /// <summary>
        /// Throws an InvalidOperationException if this <see cref="MethodExpressionBase"/> has the
        /// same signature as another method on the
        /// <see cref="MemberExpression.DeclaringTypeExpression"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this <see cref="MethodExpressionBase"/> has the same signature as another
        /// method on the <see cref="MemberExpression.DeclaringTypeExpression"/>.
        /// </exception>
        protected void ThrowIfDuplicateSignature()
        {
            if (SiblingMethodExpressions == null)
            {
                return;
            }

            var hasDuplicateMethod = SiblingMethodExpressions
                .Any(m => m.Name == Name && HasSameParameterTypesAs(m));

            if (hasDuplicateMethod)
            {
                throw new InvalidOperationException(
                    $"Type {DeclaringTypeExpression.Name} has duplicate " +
                    $"{MethodTypeName.ToLowerInvariant()} signature " +
                    $"'{GetSignature(includeTypeName: false)}'");
            }
        }

        /// <summary>
        /// When overridden in a derived type, gets the set of <see cref="MethodExpressionBase"/>s
        /// with which to verify that this <see cref="MethodExpressionBase"/> has a unique signature.
        /// </summary>
        protected abstract IEnumerable<MethodExpressionBase> SiblingMethodExpressions { get; }

        /// <summary>
        /// When overridden in a derived type, gets the name of the type of method represented by
        /// this <see cref="MethodExpressionBase"/>, e.g. 'method' or 'constructor'.
        /// </summary>
        protected abstract string MethodTypeName { get; }

        /// <summary>
        /// Determines if this <see cref="MethodExpressionBase"/> has the same paramter Types as
        /// the given <paramref name="otherMethod"/>.
        /// </summary>
        /// <param name="otherMethod">
        /// The other <see cref="MethodExpressionBase"/> for which to make the determination.
        /// </param>
        /// <returns>
        /// True if this <see cref="MethodExpressionBase"/> has the same paramter Types as the given
        /// <paramref name="otherMethod"/>, otherwise false.
        /// </returns>
        protected bool HasSameParameterTypesAs(MethodExpressionBase otherMethod)
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

        #region Helper Classes

        private class MethodParameterInfo :
            AttributableExpressionBase,
            IParameterExpressionConfigurator,
            IParameter
        {
            private ParameterExpression _parameter;
            private Type _parameterType;
            private IType _type;

            public MethodParameterInfo(ParameterExpression parameter)
            {
                _parameter = parameter;
                _parameterType = parameter.Type;
            }

            IType IParameter.Type
                => _type ??= ClrTypeWrapper.For(_parameterType);

            public string Name => _parameter.Name ?? "Unnamed";

            public bool IsOut { get; private set; }

            public bool IsRef { get; private set; }

            public bool IsParamsArray { get; private set; }

            #region IParameterExpressionConfigurator Members

            void IParameterExpressionConfigurator.SetOut()
            {
                if (IsOut)
                {
                    return;
                }

                ThrowIfRefParameter("out");
                ThrowIfParamsArray("out");

                IsOut = true;
                UpdateParameterType();
            }

            void IParameterExpressionConfigurator.SetRef()
            {
                if (IsRef)
                {
                    return;
                }

                ThrowIfOutParameter("ref");
                ThrowIfParamsArray("ref");

                IsRef = true;
                UpdateParameterType();
            }

            void IParameterExpressionConfigurator.SetParamsArray()
            {
                if (IsParamsArray)
                {
                    return;
                }

                ThrowIfInvalidForParamsArray();
                IsParamsArray = true;
            }

            private void UpdateParameterType()
            {
                _parameterType = _parameter.Type.MakeByRefType();
                _parameter = Parameter(_parameterType, Name);
                _type = null;
            }

            private void ThrowIfInvalidForParamsArray()
            {
                ThrowIfOutParameter("params");
                ThrowIfRefParameter("params");

                if (!_parameterType.IsArray)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{Name}' cannot be a params array as it " +
                        $"has type '{_parameterType.GetFriendlyName()}'.");
                }
            }

            private void ThrowIfRefParameter(string conflictingModifier)
            {
                if (IsRef)
                {
                    ThrowModifierConflict("ref", conflictingModifier);
                }
            }

            private void ThrowIfOutParameter(string conflictingModifier)
            {
                if (IsOut)
                {
                    ThrowModifierConflict("out", conflictingModifier);
                }
            }

            private void ThrowIfParamsArray(string conflictingModifier)
            {
                if (IsParamsArray)
                {
                    ThrowModifierConflict("params", conflictingModifier);
                }
            }

            private void ThrowModifierConflict(string modifier, string conflictingModifier)
            {
                throw new InvalidOperationException(
                    $"Parameter '{Name}' cannot be both {modifier} and {conflictingModifier}.");
            }

            #endregion 
        }

        #endregion
    }
}