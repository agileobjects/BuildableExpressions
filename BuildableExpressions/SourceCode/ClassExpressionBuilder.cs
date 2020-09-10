namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using ReadableExpressions;

    internal class ClassExpressionBuilder : IClassExpressionSettings
    {
        private readonly IList<MethodExpressionBuilder> _methodBuilders;
        private readonly CommentExpression _summary;
        private List<Type> _interfaceTypes;

        public ClassExpressionBuilder(string name, CommentExpression summary)
        {
            Name = name;
            _summary = summary;
            _methodBuilders = new List<MethodExpressionBuilder>();
        }

        public string Name { get; }

        public IClassExpressionSettings Implementing<TInterface>() where TInterface : class
            => Implementing(typeof(TInterface));

        public IClassExpressionSettings Implementing(params Type[] interfaces)
            => AddInterfaces(interfaces);

        private ClassExpressionBuilder AddInterfaces(IEnumerable<Type> interfaceTypes)
        {
            if (_interfaceTypes == null)
            {
                _interfaceTypes = new List<Type>(interfaceTypes);
            }
            else
            {
                _interfaceTypes.AddRange(interfaceTypes.Except(_interfaceTypes));
            }

            return this;
        }

        IClassExpressionSettings IClassExpressionSettings.WithMethod(Expression body)
            => AddMethod(name: null, summary: null, body, allowNullName: true);

        IClassExpressionSettings IClassExpressionSettings.WithMethod(
            string name,
            Expression body)
        {
            return AddMethod(name, summary: null, body);
        }

        IClassExpressionSettings IClassExpressionSettings.WithMethod(
            string name,
            string summary,
            Expression body)
        {
            if (string.IsNullOrEmpty(summary))
            {
                throw new ArgumentException(
                    "Null or empty method summary supplied",
                    nameof(summary));
            }

            return AddMethod(name, ReadableExpression.Comment(summary), body);
        }

        public IClassExpressionSettings WithMethod(
            string name,
            CommentExpression summary,
            Expression body)
        {
            return AddMethod(name, summary, body);
        }

        private IClassExpressionSettings AddMethod(
            string name,
            CommentExpression summary,
            Expression body,
            bool allowNullName = false)
        {
            name.ThrowIfInvalidName<ArgumentException>("Method", allowNullName);

            if (!allowNullName && _methodBuilders.Any(mb => mb.Name == name))
            {
                throw new InvalidOperationException(
                    $"Duplicate method name '{name}' specified.");
            }

            _methodBuilders.Add(new MethodExpressionBuilder(name, summary, body));
            return this;
        }

        public void Validate()
        {
            if (_methodBuilders.Any())
            {
                return;
            }

            var name = Name != null ? $" for class '{Name}'" : null;

            throw new InvalidOperationException(
                "At least one method must be specified" + name);
        }

        public ClassExpression Build(
            SourceCodeExpression parent,
            SourceCodeTranslationSettings settings)
        {
            return new ClassExpression(
                parent,
                Name,
                _interfaceTypes,
                _summary,
                _methodBuilders,
                settings);
        }
    }
}