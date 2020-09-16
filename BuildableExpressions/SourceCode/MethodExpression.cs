﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;
    using static MethodVisibility;

    /// <summary>
    /// Represents a method in a class in a piece of source code.
    /// </summary>
    public class MethodExpression : Expression, IMethodNamingContext
    {
        private readonly SourceCodeTranslationSettings _settings;
        private readonly MethodExpressionMethod _method;
        private List<MethodParameterExpression> _parameters;
        private ReadOnlyCollection<MethodParameterExpression> _readOnlyParameters;

        private MethodExpression(
            ClassExpression parent,
            MethodVisibility visibility,
            string name,
            CommentExpression summary,
            LambdaExpression definition,
            SourceCodeTranslationSettings settings)
        {
            Parent = parent;
            Visibility = visibility;
            Summary = summary;
            Definition = definition;
            _settings = settings;

            List<IParameter> parameters;

            var parameterCount = definition.Parameters.Count;

            if (parameterCount != 0)
            {
                _parameters = new List<MethodParameterExpression>(parameterCount);
                parameters = new List<IParameter>(parameterCount);

                for (var i = 0; i < parameterCount; ++i)
                {
                    var parameter = new MethodParameterExpression(definition.Parameters[i]);
                    parameters.Add(parameter);
                    _parameters.Add(parameter);
                }
            }
            else
            {
                _parameters = Enumerable<MethodParameterExpression>.EmptyList;
                parameters = Enumerable<IParameter>.EmptyList;
            }

            _method = new MethodExpressionMethod(
                this,
                visibility,
                name,
                parameters,
                settings);
        }

        #region Factory Methods

        internal static MethodExpression For(
            ClassExpression parent,
            Expression expression,
            SourceCodeTranslationSettings settings,
            MethodVisibility visibility = MethodVisibility.Public)
        {
            return For(
                parent,
                visibility,
                name: null,
                summary: null,
                expression,
                settings);
        }

        internal static MethodExpression For(
            ClassExpression parent,
            MethodVisibility visibility,
            string name,
            CommentExpression summary,
            Expression expression,
            SourceCodeTranslationSettings settings)
        {
            var definition = expression.ToLambdaExpression();

            return new MethodExpression(
                parent,
                visibility,
                name,
                summary,
                definition,
                settings);
        }

        #endregion

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
        /// Gets the <see cref="MethodVisibility"/> of this <see cref="MethodExpression"/>.
        /// </summary>
        public MethodVisibility Visibility { get; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="MethodExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; }

        /// <summary>
        /// Gets the name of this <see cref="MethodExpression"/>.
        /// </summary>
        public string Name => Method.Name;

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
        public LambdaExpression Definition { get; }

        /// <summary>
        /// Gets the Expression describing the body of this <see cref="MethodExpression"/>.
        /// </summary>
        public Expression Body => Definition.Body;

        internal void AddParameters(IList<ParameterExpression> parameters)
        {
            var parameterCount = parameters.Count;
            var methodParameters = new MethodParameterExpression[parameterCount];
            var iParameters = new IParameter[parameterCount];

            for (var i = 0; i < parameterCount; ++i)
            {
                iParameters[i] = methodParameters[i] =
                    new MethodParameterExpression(parameters[i]);
            }

            if (_parameters.Count == 0)
            {
                _parameters = new List<MethodParameterExpression>();
            }

            _parameters.AddRange(methodParameters);
            _readOnlyParameters = null;

            _method.AddParameters(iParameters);
        }

        internal IMethod Method => _method;

        #region IMethodNamingContext Members

        Type IMethodNamingContext.ReturnType => Type;

        string IMethodNamingContext.ReturnTypeName
            => Type.GetVariableNameInPascalCase(_settings);

        LambdaExpression IMethodNamingContext.MethodLambda => Definition;

        int IMethodNamingContext.Index => Parent?.Methods.IndexOf(this) ?? 0;

        #endregion

        internal class MethodExpressionMethod : IMethod
        {
            private readonly MethodExpression _method;
            private readonly MethodVisibility _visibility;
            private readonly SourceCodeTranslationSettings _settings;
            private string _name;
            private List<IParameter> _parameters;

            public MethodExpressionMethod(
                MethodExpression method,
                MethodVisibility visibility,
                string name,
                List<IParameter> parameters,
                SourceCodeTranslationSettings settings)
            {
                _method = method;
                _visibility = visibility;
                _name = name;
                _parameters = parameters;
                _settings = settings;
            }

            private ClassExpression Parent => _method.Parent;

            public Type DeclaringType => null;

            public bool IsPublic => _visibility == Public;

            public bool IsProtectedInternal => _visibility == ProtectedInternal;

            public bool IsInternal => _visibility == Internal;

            public bool IsProtected => _visibility == Protected;

            public bool IsPrivate => _visibility == Private;

            public bool IsAbstract => false;

            public bool IsStatic => false;

            public bool IsVirtual => false;

            public string Name => _name ??= GetName();

            private string GetName()
            {
                return _settings
                    .MethodNameFactory
                    .Invoke(Parent?.Parent, Parent, _method)
                    .ThrowIfInvalidName<InvalidOperationException>("Method");
            }

            public bool IsGenericMethod => false;

            public bool IsExtensionMethod => false;

            public Type ReturnType => _method.Type;

            public IMethod GetGenericMethodDefinition() => null;

            public Type[] GetGenericArguments() => Enumerable<Type>.EmptyArray;

            public IList<IParameter> GetParameters() => _parameters;

            public void AddParameters(IList<IParameter> parameters)
            {
                if (_parameters.Count == 0)
                {
                    _parameters = new List<IParameter>(parameters.Count);
                }

                _parameters.AddRange(parameters);
            }
        }
    }
}