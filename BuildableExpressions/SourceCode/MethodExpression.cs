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
    using ReadableExpressions.Translations.Reflection;
    using static MemberVisibility;

    /// <summary>
    /// Represents a method in a class in a piece of source code.
    /// </summary>
    public class MethodExpression :
        Expression,
        IMethodNamingContext,
        IMethodExpressionConfigurator,
        IMethod
    {
        private readonly SourceCodeTranslationSettings _settings;
        private List<MethodParameterExpression> _parameters;
        private List<IParameter> _methodParameters;
        private ReadOnlyCollection<MethodParameterExpression> _readOnlyParameters;
        private string _name;

        internal MethodExpression(
            ClassExpression parent,
            Expression body,
            SourceCodeTranslationSettings settings)
        {
            Parent = parent;
            Definition = body.ToLambdaExpression();
            _settings = settings;

            var parameterCount = Definition.Parameters.Count;

            if (parameterCount == 0)
            {
                _parameters = Enumerable<MethodParameterExpression>.EmptyList;
                _methodParameters = Enumerable<IParameter>.EmptyList;
                return;
            }

            _parameters = new List<MethodParameterExpression>(parameterCount);
            _methodParameters = new List<IParameter>(parameterCount);

            for (var i = 0; i < parameterCount; ++i)
            {
                var parameter = new MethodParameterExpression(Definition.Parameters[i]);
                _methodParameters.Add(parameter);
                _parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1002) indicating the type of this
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

        /// <summary>
        /// Gets this <see cref="MethodExpression"/>'s parent <see cref="ClassExpression"/>.
        /// </summary>
        public ClassExpression Parent { get; }

        /// <summary>
        /// Gets the <see cref="MemberVisibility"/> of this <see cref="MethodExpression"/>.
        /// </summary>
        public MemberVisibility Visibility { get; private set; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="MethodExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="MethodExpression"/>.
        /// </summary>
        public string Name => _name ??= GetName();

        private string GetName()
        {
            return _settings
                .MethodNameFactory
                .Invoke(Parent?.Parent, Parent, this)
                .ThrowIfInvalidName<InvalidOperationException>("Method");
        }

        /// <summary>
        /// Gets the return type of this <see cref="MethodExpression"/>, which is the return type
        /// of the LambdaExpression from which the method was created.
        /// </summary>
        public Type ReturnType => Definition.ReturnType;

        /// <summary>
        /// Gets the <see cref="MethodParameterExpression"/>s describing the parameters of this
        /// <see cref="MethodExpression"/>.
        /// </summary>
        public ReadOnlyCollection<MethodParameterExpression> Parameters
            => _readOnlyParameters ??= _parameters.ToReadOnlyCollection();

        /// <summary>
        /// Gets the LambdaExpression describing the parameters and body of this
        /// <see cref="MethodExpression"/>.
        /// </summary>
        public LambdaExpression Definition { get; private set; }

        /// <summary>
        /// Gets the Expression describing the body of this <see cref="MethodExpression"/>.
        /// </summary>
        public Expression Body => Definition.Body;

        internal void Finalise(
            Expression body,
            IList<MethodParameterExpression> parameters)
        {
            var parametersUnchanged = _parameters.SequenceEqual(parameters);

            if (parametersUnchanged && body == Body)
            {
                return;
            }

            Definition = Definition.Update(
                body,
                parametersUnchanged
                    ? (IEnumerable<ParameterExpression>)Definition.Parameters
                    : parameters.ProjectToArray(p => p.ParameterExpression));

            if (parametersUnchanged)
            {
                return;
            }

            var parameterCount = parameters.Count;

            if (_parameters.Count == 0)
            {
                _parameters = new List<MethodParameterExpression>(parameterCount);
                _methodParameters = new List<IParameter>(parameterCount);
            }
            else
            {
                _parameters.Clear();
                _methodParameters.Clear();
            }

            _parameters.AddRange(parameters);
            _methodParameters.AddRange(parameters);
            _readOnlyParameters = null;
        }

        #region IMethodNamingContext Members

        Type IMethodNamingContext.ReturnType => Type;

        string IMethodNamingContext.ReturnTypeName
            => Type.GetVariableNameInPascalCase(_settings);

        LambdaExpression IMethodNamingContext.MethodLambda => Definition;

        int IMethodNamingContext.Index => Parent?.Methods.IndexOf(this) ?? 0;

        #endregion

        #region IMethodExpressionConfigurator Members

        IMethodExpressionConfigurator IMethodExpressionConfigurator.WithSummary(
            string summary)
        {
            Summary = ReadableExpression.Comment(summary);
            return this;
        }

        IMethodExpressionConfigurator IMethodExpressionConfigurator.WithSummary(
            CommentExpression summary)
        {
            Summary = summary;
            return this;
        }

        IMethodExpressionConfigurator IMethodExpressionConfigurator.Named(
            string name)
        {
            _name = name.ThrowIfInvalidName<ArgumentException>("Method");
            return this;
        }

        IMethodExpressionConfigurator IMethodExpressionConfigurator.WithVisibility(
            MemberVisibility visibility)
        {
            Visibility = visibility;
            return this;
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

        bool IMethod.IsStatic => false;

        bool IMethod.IsVirtual => false;

        bool IMethod.IsGenericMethod => false;

        bool IMethod.IsExtensionMethod => false;

        IMethod IMethod.GetGenericMethodDefinition() => null;

        Type[] IMethod.GetGenericArguments() => Enumerable<Type>.EmptyArray;

        IList<IParameter> IMethod.GetParameters() => _methodParameters;

        #endregion
    }
}