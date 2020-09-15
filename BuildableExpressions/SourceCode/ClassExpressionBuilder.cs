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
        private CommentExpression _summary;
        private List<Type> _interfaceTypes;

        public ClassExpressionBuilder(string name)
        {
            Name = name;
            _methodBuilders = new List<MethodExpressionBuilder>();
        }

        public string Name { get; }

        public IClassExpressionSettings Implementing<TInterface>() where TInterface : class
            => Implementing(typeof(TInterface));

        public IClassExpressionSettings Implementing(params Type[] interfaces)
            => AddInterfaces(interfaces);

        public IClassExpressionSettings WithSummary(string summary)
            => WithSummary(ReadableExpression.Comment(summary));

        public IClassExpressionSettings WithSummary(CommentExpression summary)
        {
            _summary = summary;
            return this;
        }

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

        public IClassExpressionSettings WithMethod(Expression body)
            => AddMethod(name: null, body, throwIfNullName: false);

        public IClassExpressionSettings WithMethod(
            string name,
            Expression body)
        {
            return AddMethod(name, body);
        }

        public IClassExpressionSettings WithMethod(
            string name,
            Expression body,
            Func<IMethodExpressionSettings, IMethodExpressionSettings> configuration)
        {
            return AddMethod(name, body, configuration);
        }

        private IClassExpressionSettings AddMethod(
            string name,
            Expression body,
            Func<IMethodExpressionSettings, IMethodExpressionSettings> configuration = null,
            bool throwIfNullName = true)
        {
            name.ThrowIfInvalidName<ArgumentException>("Method", throwIfNullName);

            if (throwIfNullName && _methodBuilders.Any(mb => mb.Name == name))
            {
                throw new InvalidOperationException(
                    $"Duplicate method name '{name}' specified.");
            }

            var builder = new MethodExpressionBuilder(name, body);
            configuration?.Invoke(builder);

            _methodBuilders.Add(builder);
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