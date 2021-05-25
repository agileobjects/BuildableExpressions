namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using Extensions;
    using NetStandardPolyfills;

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
            ConstructorExpression = AttributeExpression is ITypedTypeExpression
                ? new ConstructorInfoConstructorExpression(
                    AttributeExpression,
                    AttributeExpression.Type.GetPublicInstanceConstructors().First())
                : AttributeExpression.ConstructorExpressions.First();

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

        #endregion
    }
}