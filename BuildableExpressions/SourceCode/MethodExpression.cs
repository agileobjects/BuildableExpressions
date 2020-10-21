namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;
    using Translations;
    using static MemberVisibility;

    /// <summary>
    /// Represents a method in a type in a piece of source code.
    /// </summary>
    public class MethodExpression :
        Expression,
        IMethodExpressionConfigurator,
        IMethod,
        ICustomAnalysableExpression,
        ICustomTranslationExpression
    {
        private List<GenericParameterExpression> _genericArguments;
        private ReadOnlyCollection<GenericParameterExpression> _readonlyGenericParameters;
        private ReadOnlyCollection<IGenericArgument> _readonlyGenericArguments;
        private ReadOnlyCollection<IParameter> _parameters;
        private string _name;

        internal MethodExpression(
            TypeExpression typeExpression,
            string name,
            Expression body,
            Action<IMethodExpressionConfigurator> configuration)
        {
            DeclaringTypeExpression = typeExpression;
            _name = name;
            Definition = body.ToLambdaExpression();

            typeExpression.Register(this);
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
        public string Name => _name ??= DeclaringTypeExpression.GetMethodName(this);

        /// <summary>
        /// Gets the return type of this <see cref="MethodExpression"/>, which is the return type
        /// of the LambdaExpression from which the method was created.
        /// </summary>
        public Type ReturnType => Definition.ReturnType;

        /// <summary>
        /// Gets the LambdaExpression describing the parameters and body of this
        /// <see cref="MethodExpression"/>.
        /// </summary>
        public LambdaExpression Definition { get; private set; }

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

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/>s describing the parameters of this
        /// <see cref="MethodExpression"/>, if any.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Parameters
            => Definition.Parameters;

        /// <summary>
        /// Gets the Expression describing the body of this <see cref="MethodExpression"/>.
        /// </summary>
        public Expression Body => Definition.Body;

        #region IMethodExpressionConfigurator Members

        void IMethodExpressionConfigurator.SetSummary(CommentExpression summary)
            => Summary = summary;

        void IMethodExpressionConfigurator.SetVisibility(MemberVisibility visibility)
            => Visibility = visibility;

        void IMethodExpressionConfigurator.SetStatic()
            => IsStatic = true;

        void IMethodExpressionConfigurator.AddGenericParameters(
            params GenericParameterExpression[] parameters)
        {
            _genericArguments ??= new List<GenericParameterExpression>();
            _readonlyGenericParameters = null;
            _readonlyGenericArguments = null;

            foreach (var parameter in parameters)
            {
                _genericArguments.Add(parameter);
                parameter.SetMethod(this);
            }
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
            return _parameters ??= Parameters
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

        internal void Validate()
        {
            if (IsGeneric)
            {
                ThrowIfDuplicateGenericArgumentNames();
            }
        }

        private void ThrowIfDuplicateGenericArgumentNames()
        {
            if (!(_genericArguments?.Count > 1))
            {
                return;
            }

            var duplicateParameterName = _genericArguments
                .GroupBy(arg => arg.Name)
                .FirstOrDefault(nameGroup => nameGroup.Count() > 1)?
                .Key;

            if (duplicateParameterName != null)
            {
                throw new InvalidOperationException(
                    $"Method '{DeclaringTypeExpression.Name}.{Name}': " +
                    $"duplicate generic parameter name '{duplicateParameterName}' specified.");
            }
        }

        internal void Finalise(
            Expression body,
            IList<ParameterExpression> parameters)
        {
            var parametersUnchanged = Parameters.SequenceEqual(parameters);

            if (parametersUnchanged)
            {
                if (body == Body)
                {
                    return;
                }

                parameters = Definition.Parameters;
                _parameters = null;
            }

            Definition = Definition.Update(body, parameters);
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