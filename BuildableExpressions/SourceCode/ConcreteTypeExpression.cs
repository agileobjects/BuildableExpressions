namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;
    using Generics;

    /// <summary>
    /// Represents a concrete (non-interface) type in a piece of source code.
    /// </summary>
    public abstract class ConcreteTypeExpression :
        TypeExpression,
        IConcreteTypeExpressionConfigurator
    {
        private Expression _thisInstanceExpression;

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

        internal Expression ThisInstanceExpression
            => _thisInstanceExpression ??= InstanceExpression.This(this);

        internal override void SetImplements(
            Type @interface,
            Action<ImplementationConfigurator> configuration)
        {
            base.SetImplements(@interface, configuration);
            ThrowIfInterfaceMethodNotImplemented(@interface);
        }

        #region IConcreteTypeExpressionConfigurator Members

        Expression IConcreteTypeExpressionConfigurator.ThisInstanceExpression
            => ThisInstanceExpression;

        void IConcreteTypeExpressionConfigurator.AddGenericParameter(
            GenericParameterExpression parameter)
        {
            AddGenericParameter(parameter);
        }

        #endregion
    }
}