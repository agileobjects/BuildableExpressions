namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api;

    internal class SourceCodeExpressionBuilder :
        SourceCodeTranslationSettings,
        ISourceCodeExpressionSettings
    {
        private readonly IList<ClassExpressionBuilder> _classBuilders;

        public SourceCodeExpressionBuilder()
        {
            SetDefaultSourceCodeOptions(this);

            _classBuilders = new List<ClassExpressionBuilder>();
        }

        ISourceCodeExpressionSettings ISourceCodeExpressionSettings.WithNamespaceOf<T>()
            => SetNamespaceTo(typeof(T).Namespace);

        ISourceCodeExpressionSettings ISourceCodeExpressionSettings.WithNamespaceOf(Type type)
            => SetNamespaceTo(type.Namespace);

        ISourceCodeExpressionSettings ISourceCodeExpressionSettings.WithNamespace(string @namespace)
            => SetNamespaceTo(@namespace);

        private ISourceCodeExpressionSettings SetNamespaceTo(string @namespace)
        {
            SetNamespace(@namespace);
            return this;
        }

        ISourceCodeExpressionSettings ISourceCodeExpressionSettings.WithClass(
            Func<IClassExpressionSettings, IClassExpressionSettings> configuration)
        {
            return AddClass(name: null, configuration, throwIfNullName: false);
        }

        ISourceCodeExpressionSettings ISourceCodeExpressionSettings.WithClass(
            string name,
            Func<IClassExpressionSettings, IClassExpressionSettings> configuration)
        {
            return AddClass(name, configuration);
        }

        private ISourceCodeExpressionSettings AddClass(
            string name,
            Func<IClassExpressionSettings, IClassExpressionSettings> configuration,
            bool throwIfNullName = true)
        {
            name.ThrowIfInvalidName<ArgumentException>("Class", throwIfNullName);

            if (throwIfNullName && _classBuilders.Any(b => b.Name == name))
            {
                throw new InvalidOperationException(
                    $"Duplicate class name '{name}' specified.");
            }

            var builder = new ClassExpressionBuilder(name);
            configuration.Invoke(builder);

            builder.Validate();
            _classBuilders.Add(builder);
            return this;
        }

        public SourceCodeExpression Build()
        {
            if (!_classBuilders.Any())
            {
                throw new InvalidOperationException(
                    "At least one class must be specified");
            }

            return new SourceCodeExpression(_classBuilders, this);
        }
    }
}