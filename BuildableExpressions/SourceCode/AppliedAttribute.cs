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
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using static System.Environment;

    /// <summary>
    /// Represents the application of an <see cref="AttributeExpression"/> to a source code element.
    /// </summary>
    public class AppliedAttribute : IAttributeApplicationConfigurator
    {
        private IList<ConstantExpression> _arguments;
        private ReadOnlyCollection<ConstantExpression> _readOnlyArguments;

        internal AppliedAttribute(
            AttributeExpression attributeExpression)
        {
            AttributeExpression = attributeExpression;
        }

        /// <summary>
        /// Gets the <see cref="AttributeExpression"/> describing the applied attribute.
        /// </summary>
        public AttributeExpression AttributeExpression { get; }

        /// <summary>
        /// Gets the <see cref="ConstructorExpression"/> describing the
        /// <see cref="AttributeExpression"/> constructor called with the defined
        /// <see cref="Arguments"/>, if arguments are being passed. If no arguments are being
        /// passed, returns null.
        /// </summary>
        public ConstructorExpression ConstructorExpression { get; private set; }

        /// <summary>
        /// Gets the values being passed to the constructor of the applied
        /// <see cref="AttributeExpression"/>.
        /// </summary>
        public ReadOnlyCollection<ConstantExpression> Arguments
            => _readOnlyArguments ??= _arguments.ToReadOnlyCollection();

        internal ICollection<ConstantExpression> ArgumentsAccessor => _arguments;

        #region IAttributeApplicationConfigurator Members

        void IAttributeApplicationConfigurator.SetConstructorArguments(
            params object[] arguments)
        {
            ConstructorExpression = FindConstructorOrThrow(AttributeExpression, arguments);

            var parameters = ConstructorExpression.Parameters;
            var argumentCount = arguments.Length;
            _arguments = new ConstantExpression[argumentCount];

            for (var i = 0; i < argumentCount; i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];

                if (argument == null)
                {
                    _arguments[i] = Expression.Constant(null, parameter.Type);
                    continue;
                }

                var typedArgument = Convert.ChangeType(argument, parameter.Type);
                _arguments[i] = Expression.Constant(typedArgument, parameter.Type);
            }
        }

        private static ConstructorExpression FindConstructorOrThrow(
            TypeExpression attribute,
            IList<object> arguments)
        {
            if (attribute is ITypedTypeExpression)
            {
                return new ConstructorInfoConstructorExpression(
                    attribute,
                    FindConstructorOrThrow(
                        attribute,
                        attribute.Type.GetPublicInstanceConstructors(),
                        ctorInfo => ctorInfo.GetParameters().Project(p => p.ParameterType),
                        arguments));
            }

            return FindConstructorOrThrow(
                attribute,
                attribute.ConstructorExpressions,
                ctor => ctor.ParametersAccessor?.Project(p => p.Type) ?? Type.EmptyTypes,
                arguments);
        }

        private static TCtor FindConstructorOrThrow<TCtor>(
            TypeExpression attribute,
            IEnumerable<TCtor> constructors,
            Func<TCtor, IEnumerable<Type>> ctorParameterTypesAccessor,
            IList<object> arguments)
        {
            var argumentTypes = arguments.ProjectToArray(arg => arg?.GetType());

            foreach (var constructor in constructors)
            {
                var parameterTypes =
                    ctorParameterTypesAccessor.Invoke(constructor);

                var i = 0;

                foreach (var parameterType in parameterTypes)
                {
                    var argumentType = argumentTypes[i];
                    ++i;

                    if (argumentType == null)
                    {
                        if (parameterType.CanBeNull())
                        {
                            continue;
                        }

                        --i;
                        break;
                    }

                    if (!argumentType.IsAssignableTo(parameterType))
                    {
                        break;
                    }
                }

                if (i == arguments.Count)
                {
                    return constructor;
                }
            }

            var givenArgumentTypeNames = argumentTypes.Project(t => t?.GetFriendlyName() ?? "null");

            var validParameterTypes = constructors
                .Project(ctorParameterTypesAccessor.Invoke)
                .Project(pt => string.Join(", ", pt.Project(t => t.GetFriendlyName())))
                .ToList();

            var availableCtor = validParameterTypes.Any()
                ? $"Available constructor(s) are:{NewLine}  - " +
                  string.Join(NewLine + "  - ", validParameterTypes)
                  : "Only a parameterless constructor is available";

            throw new ArgumentException(
                $"Unable to find '{attribute.GetFriendlyName()}' constructor " +
                $"matching argument type(s) '{string.Join(", ", givenArgumentTypeNames)}'. " +
                 availableCtor);
        }

        #endregion
    }
}