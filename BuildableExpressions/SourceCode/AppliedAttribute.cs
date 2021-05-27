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
    using ReadableExpressions.Translations.Reflection;
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

        internal bool AllowMultiple { get; set; }

        #region IAttributeApplicationConfigurator Members

        void IAttributeApplicationConfigurator.SetConstructorArguments(
            params object[] arguments)
        {
            arguments ??= new object[] { null };

            ConstructorExpression = FindSingleConstructorOrThrow(AttributeExpression, arguments);

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

        private static ConstructorExpression FindSingleConstructorOrThrow(
            TypeExpression attribute,
            IList<object> arguments)
        {
            if (attribute is ITypedTypeExpression)
            {
                return new ConstructorInfoConstructorExpression(
                    attribute,
                    FindSingleConstructorOrThrow(
                        attribute,
                        attribute.Type.GetPublicInstanceConstructors(),
                        ctorInfo => ctorInfo.GetParameters().Project(p => p.ParameterType),
                        arguments));
            }

            return FindSingleConstructorOrThrow(
                attribute,
                attribute.ConstructorExpressions,
                ctor => ctor.ParametersAccessor?.Project(p => p.Type) ?? Type.EmptyTypes,
                arguments);
        }

        private static TCtor FindSingleConstructorOrThrow<TCtor>(
            IType attribute,
            IEnumerable<TCtor> constructors,
            Func<TCtor, IEnumerable<Type>> ctorParameterTypesAccessor,
            IList<object> arguments)
        {
            var argumentTypes = arguments.ProjectToArray(arg => arg?.GetType());
            var argumentCount = arguments.Count;
            var matchingConstructors = new List<TCtor>();

            foreach (var constructor in constructors)
            {
                var parameterTypes = ctorParameterTypesAccessor
                    .Invoke(constructor)
                    .ToList();

                if (parameterTypes.Count != argumentCount)
                {
                    continue;
                }

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

                if (i == argumentCount)
                {
                    matchingConstructors.Add(constructor);
                }
            }

            switch (matchingConstructors.Count)
            {
                case 0:
                    throw ConstructorNotFound(
                        attribute,
                        constructors,
                        ctorParameterTypesAccessor,
                        argumentTypes);

                case 1:
                    return matchingConstructors[0];

                default:
                    throw ConstructorAmbiguous(
                        attribute,
                        matchingConstructors,
                        ctorParameterTypesAccessor,
                        argumentTypes);
            }
        }

        private static Exception ConstructorAmbiguous<TCtor>(
            IType attribute,
            IEnumerable<TCtor> matchingConstructors,
            Func<TCtor, IEnumerable<Type>> ctorParameterTypesAccessor,
            IEnumerable<Type> argumentTypes)
        {
            var matchingCtorParameterTypes = GetConstructorParameterTypes(
                GetConstructorParameterTypes(matchingConstructors, ctorParameterTypesAccessor));

            return new ArgumentException(
                $"Multiple '{attribute.GetFriendlyName()}' constructors " +
                $"match argument type(s) '{GetArgumentTypeNames(argumentTypes)}':" +
                 matchingCtorParameterTypes);
        }

        private static Exception ConstructorNotFound<TCtor>(
            IType attribute,
            IEnumerable<TCtor> constructors,
            Func<TCtor, IEnumerable<Type>> ctorParameterTypesAccessor,
            IEnumerable<Type> argumentTypes)
        {
            var validParameterTypes =
                GetConstructorParameterTypes(constructors, ctorParameterTypesAccessor);

            var availableCtor = validParameterTypes.Any()
                ? "Available constructor(s) are:" + GetConstructorParameterTypes(validParameterTypes)
                : "Only a parameterless constructor is available";

            return new ArgumentException(
                $"Unable to find '{attribute.GetFriendlyName()}' constructor " +
                $"matching argument type(s) '{GetArgumentTypeNames(argumentTypes)}'. " +
                availableCtor);
        }

        private static string GetArgumentTypeNames(IEnumerable<Type> argumentTypes)
            => string.Join(", ", argumentTypes.Project(t => t?.GetFriendlyName() ?? "null"));

        private static ICollection<string> GetConstructorParameterTypes<TCtor>(
            IEnumerable<TCtor> constructors,
            Func<TCtor, IEnumerable<Type>> ctorParameterTypesAccessor)
        {
            return constructors
                .Project(ctorParameterTypesAccessor.Invoke)
                .Project(pt => string.Join(", ", pt.Project(t => t.GetFriendlyName())))
                .ToList();
        }

        private static string GetConstructorParameterTypes(IEnumerable<string> ctorParameterTypes)
        {
            var separator = NewLine + "  - (";
            return separator + string.Join(")" + separator, ctorParameterTypes) + ")";
        }

        #endregion
    }
}