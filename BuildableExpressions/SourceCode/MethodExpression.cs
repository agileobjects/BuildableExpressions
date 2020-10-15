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
    /// Represents a method in a class in a piece of source code.
    /// </summary>
    public class MethodExpression :
        Expression,
        IMethodNamingContext,
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
            ClassExpression @class,
            Expression body,
            SourceCodeTranslationSettings settings)
        {
            Class = @class;
            Definition = body.ToLambdaExpression();
            Settings = settings;
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

        internal SourceCodeTranslationSettings Settings { get; }

        internal MethodExpressionAnalysis Analysis { get; set; }

        /// <summary>
        /// Gets this <see cref="MethodExpression"/>'s parent <see cref="ClassExpression"/>.
        /// </summary>
        public ClassExpression Class { get; }

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
        /// Gets the name of this <see cref="MethodExpression"/>.
        /// </summary>
        public string Name => _name ??= GetName();

        private string GetName()
        {
            return Settings
                .MethodNameFactory
                .Invoke(Class.SourceCode, Class, this)
                .ThrowIfInvalidName<InvalidOperationException>("Method");
        }

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
                return _readonlyGenericParameters ??= HasGenericArguments
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

        private bool HasGenericArguments => _genericArguments?.Any() == true;

        #region IMethodNamingContext Members

        Type IMethodNamingContext.ReturnType => Type;

        string IMethodNamingContext.ReturnTypeName
            => Type.GetVariableNameInPascalCase(Settings);

        LambdaExpression IMethodNamingContext.MethodLambda => Definition;

        int IMethodNamingContext.Index => Class.Methods.IndexOf(this);

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

        IMethodExpressionConfigurator IMethodExpressionConfigurator.WithVisibility(
            MemberVisibility visibility)
        {
            Visibility = visibility;
            return this;
        }

        IMethodExpressionConfigurator IMethodExpressionConfigurator.AsStatic()
        {
            IsStatic = true;
            return this;
        }

        IMethodExpressionConfigurator IMethodExpressionConfigurator.Named(
            string name)
        {
            _name = name.ThrowIfInvalidName<ArgumentException>("Method");
            return this;
        }

        IMethodExpressionConfigurator IMethodExpressionConfigurator.WithGenericParameter(
            GenericParameterExpression parameter)
        {
            parameter.Method = this;
            (_genericArguments ??= new List<GenericParameterExpression>()).Add(parameter);
            _readonlyGenericParameters = null;
            _readonlyGenericArguments = null;
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

        bool IMethod.IsVirtual => false;

        bool IMethod.IsGenericMethod => HasGenericArguments;

        bool IMethod.IsExtensionMethod => false;

        IMethod IMethod.GetGenericMethodDefinition() => null;

        ReadOnlyCollection<IGenericArgument> IMethod.GetGenericArguments()
        {
            return _readonlyGenericArguments ??= HasGenericArguments
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