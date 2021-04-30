namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Api;
    using Generics;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents a concrete (non-interface) type in a piece of source code.
    /// </summary>
    public abstract class ConcreteTypeExpression :
        TypeExpression,
        IConcreteTypeExpressionConfigurator
    {
        private Expression _thisInstanceExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteTypeExpression"/> class for the
        /// given <paramref name="concreteType"/>.
        /// </summary>
        /// <param name="concreteType">The Type represented by the <see cref="ConcreteTypeExpression"/>.</param>
        protected ConcreteTypeExpression(Type concreteType)
            : base(concreteType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteTypeExpression"/> class.
        /// </summary>
        /// <param name="sourceCode">
        /// The <see cref="ConcreteTypeExpression"/>'s parent <see cref="SourceCodeExpression"/>.
        /// </param>
        /// <param name="name">The name of the <see cref="ConcreteTypeExpression"/>.</param>
        protected ConcreteTypeExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        internal override void SetImplements(
            InterfaceExpression interfaceExpression,
            Action<ImplementationConfigurator> configuration)
        {
            base.SetImplements(interfaceExpression, configuration);
            ThrowIfMethodNotImplemented(interfaceExpression);
        }

        private void ThrowIfMethodNotImplemented(InterfaceExpression interfaceExpression)
        {
            var unimplementedMethod = new[] { interfaceExpression }
                .Concat(interfaceExpression.InterfaceTypeExpressions)
                .SelectMany(it => it.MethodExpressions)
                .FirstOrDefault(method => !MethodExpressionsAccessor.Contains(method));

            if (unimplementedMethod == null)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Method '{unimplementedMethod.GetSignature()}' has not been implemented");
        }

        #region IConcreteTypeExpressionConfigurator Members

        Expression IConcreteTypeExpressionConfigurator.ThisInstanceExpression
            => _thisInstanceExpression ??= InstanceExpression.This(this);

        void IConcreteTypeExpressionConfigurator.AddGenericParameter(
            GenericParameterExpression parameter)
        {
            AddGenericParameter(parameter);
        }

        ConstructorExpression IConstructorConfigurator.AddConstructor(
            Action<IConstructorExpressionConfigurator> configuration)
        {
            return AddConstructor(configuration);
        }

        FieldExpression IConcreteTypeExpressionConfigurator.AddField(
            string name,
            IType type,
            Action<IFieldExpressionConfigurator> configuration)
        {
            return AddField(name, type, configuration);
        }

        #endregion
    }
}