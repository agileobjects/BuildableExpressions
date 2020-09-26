﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using BuildableExpressions.Extensions;
    using Extensions;
    using ReadableExpressions;
    using ReadableExpressions.Extensions;
    using static MemberVisibility;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public class ClassExpression : Expression, IClassNamingContext
    {
        private readonly Expression _body;
        private readonly SourceCodeTranslationSettings _settings;
        private readonly List<MethodExpression> _methods;
        private readonly Dictionary<Type, List<MethodExpression>> _methodsByReturnType;
        private ReadOnlyCollection<MethodExpression> _readOnlyMethods;
#if FEATURE_READONLYDICTIONARY
        private ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> _readOnlyMethodsByReturnType;
#else
        private IDictionary<Type, ReadOnlyCollection<MethodExpression>> _readOnlyMethodsByReturnType;
#endif
        private string _name;
        private Type _type;

        internal ClassExpression(
            SourceCodeExpression parent,
            Expression body,
            SourceCodeTranslationSettings settings)
            : this(parent, null, body, settings)
        {
        }

        internal ClassExpression(
            SourceCodeExpression parent,
            BlockExpression body,
            SourceCodeTranslationSettings settings)
            : this(parent, ClassVisibility.Public, null, settings)
        {
            Interfaces = Enumerable<Type>.EmptyReadOnlyCollection;
            _body = body;
            _methods = new List<MethodExpression>();
            _methodsByReturnType = new Dictionary<Type, List<MethodExpression>>();

            foreach (var expression in body.Expressions)
            {
                var method = MethodExpression.For(this, expression, settings);
                _methods.Add(method);
                AddTypedMethod(method);
            }
        }

        internal ClassExpression(
            SourceCodeExpression parent,
            CommentExpression summary,
            Expression body,
            SourceCodeTranslationSettings settings)
            : this(parent, ClassVisibility.Public, summary, settings)
        {
            Interfaces = Enumerable<Type>.EmptyReadOnlyCollection;
            _body = body;

            var method = MethodExpression.For(this, body, settings);
            _methods = new List<MethodExpression> { method };

            _methodsByReturnType = new Dictionary<Type, List<MethodExpression>>
            {
                { method.ReturnType, new List<MethodExpression> { method } }
            };
        }

        internal ClassExpression(
            SourceCodeExpression parent,
            ClassVisibility visibility,
            string name,
            IList<Type> interfaceTypes,
            CommentExpression summary,
            IList<MethodExpressionBuilder> methodBuilders,
            SourceCodeTranslationSettings settings)
            : this(parent, visibility, summary, settings)
        {
            _name = name;

            Interfaces = interfaceTypes != null
                ? new ReadOnlyCollection<Type>(interfaceTypes)
                : Enumerable<Type>.EmptyReadOnlyCollection;

            var methodCount = methodBuilders.Count;

            if (methodCount == 1)
            {
                var method = methodBuilders[0].Build(this, settings);
                _body = method.Definition;
                _methods = new List<MethodExpression> { method };
                _methodsByReturnType = new Dictionary<Type, List<MethodExpression>>
                {
                    { method.ReturnType, new List<MethodExpression> { method } }
                };
                return;
            }

            _methods = new List<MethodExpression>();
            _methodsByReturnType = new Dictionary<Type, List<MethodExpression>>();

            foreach (var methodBuilder in methodBuilders)
            {
                var method = methodBuilder.Build(this, settings);
                _methods.Add(method);
                AddTypedMethod(method);
            }

            _body = Block(_methods.ProjectToArray(m => (Expression)m));
        }

        private ClassExpression(
            SourceCodeExpression parent,
            ClassVisibility visibility,
            CommentExpression summary,
            SourceCodeTranslationSettings settings)
        {
            Parent = parent;
            Visibility = visibility;
            Summary = summary;
            _settings = settings;
        }

        #region Setup

        private void AddTypedMethod(MethodExpression method)
        {
            if (!_methodsByReturnType.TryGetValue(method.ReturnType, out var typedMethods))
            {
                _methodsByReturnType.Add(
                    method.ReturnType,
                    typedMethods = new List<MethodExpression>());
            }

            typedMethods.Add(method);
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1001) indicating the type of this
        /// <see cref="ClassExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.Class;

        /// <summary>
        /// Gets the type of this <see cref="ClassExpression"/>, which is the return type of the
        /// Expression from which the main method of the class was created.
        /// </summary>
        public override Type Type
            => _type ??= (_body as LambdaExpression)?.ReturnType ?? _body.Type;

        /// <summary>
        /// Visits each of this <see cref="ClassExpression"/>'s <see cref="Methods"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="ClassExpression"/>'s
        /// <see cref="Methods"/>.
        /// </param>
        /// <returns>This <see cref="ClassExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            visitor.Visit(Summary);

            foreach (var method in Methods)
            {
                visitor.Visit(method);
            }

            return this;
        }

        /// <summary>
        /// Gets this <see cref="ClassExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </summary>
        public SourceCodeExpression Parent { get; }

        /// <summary>
        /// The this <see cref="ClassExpression"/>'s <see cref="ClassVisibility">visibility</see>.
        /// </summary>
        public ClassVisibility Visibility { get; }

        /// <summary>
        /// Gets a <see cref="CommentExpression"/> describing this <see cref="ClassExpression"/>,
        /// if a summary has been set.
        /// </summary>
        public CommentExpression Summary { get; }

        /// <summary>
        /// Gets the name of this <see cref="ClassExpression"/>.
        /// </summary>
        public string Name => _name ??= GetName();

        private string GetName()
        {
            return _settings
                .ClassNameFactory
                .Invoke(Parent, this)
                .ThrowIfInvalidName<InvalidOperationException>("Class");
        }

        /// <summary>
        /// Gets the interface types implemented by this <see cref="ClassExpression"/>.
        /// </summary>
        public ReadOnlyCollection<Type> Interfaces { get; }

        /// <summary>
        /// Adds a new method for the given <paramref name="expression"/> to this
        /// <see cref="ClassExpression"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression from which to create the new <see cref="MethodExpression"/>.
        /// </param>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> of the <see cref="MethodExpression"/> to create.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        public MethodExpression AddMethod(
            Expression expression,
            MemberVisibility visibility = Public)
        {
            var method = MethodExpression
                .For(this, expression, _settings, visibility);

            _methods.Add(method);
            _readOnlyMethods = null;

            AddTypedMethod(method);
            _readOnlyMethodsByReturnType = null;

            return method;
        }

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="ClassExpression"/>'s
        /// methods.
        /// </summary>
        public ReadOnlyCollection<MethodExpression> Methods
            => _readOnlyMethods ??= _methods.ToReadOnlyCollection();

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this <see cref="ClassExpression"/>'s
        /// methods, kyed by their return type.
        /// </summary>
#if FEATURE_READONLYDICTIONARY
        public ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> MethodsByReturnType
#else
        public IDictionary<Type, ReadOnlyCollection<MethodExpression>> MethodsByReturnType
#endif
            => _readOnlyMethodsByReturnType ??= GetMethodsByReturnType();

#if FEATURE_READONLYDICTIONARY
        private ReadOnlyDictionary<Type, ReadOnlyCollection<MethodExpression>> GetMethodsByReturnType()
#else
        private IDictionary<Type, ReadOnlyCollection<MethodExpression>> GetMethodsByReturnType()
#endif
        {
            var readonlyMethodsByReturnType =
                new Dictionary<Type, ReadOnlyCollection<MethodExpression>>(_methodsByReturnType.Count);

            foreach (var methodAndReturnType in _methodsByReturnType)
            {
                readonlyMethodsByReturnType.Add(
                    methodAndReturnType.Key,
                    methodAndReturnType.Value.ToReadOnlyCollection());
            }

            return readonlyMethodsByReturnType
#if FEATURE_READONLYDICTIONARY
                    .ToReadOnlyDictionary()
#endif
                ;
        }

        /// <summary>
        /// Gets the index of this <see cref="ClassExpression"/> in the set of generated classes.
        /// </summary>
        public int Index => Parent?.Classes.IndexOf(this) ?? 0;

        #region IClassNamingContext Members

        ExpressionType IClassNamingContext.NodeType => _body.NodeType;

        string IClassNamingContext.TypeName
            => Type.GetVariableNameInPascalCase(_settings);

        Expression IClassNamingContext.Body => _body;

        #endregion
    }
}