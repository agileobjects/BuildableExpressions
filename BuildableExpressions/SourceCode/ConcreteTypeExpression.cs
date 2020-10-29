namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using Api;

    /// <summary>
    /// Represents a concrete (non-interface) type in a piece of source code.
    /// </summary>
    public abstract class ConcreteTypeExpression :
        TypeExpression,
        IConcreteTypeExpressionConfigurator
    {
        private Expression _thisInstanceExpression;

        internal ConcreteTypeExpression(SourceCodeExpression sourceCode, string name)
            : base(sourceCode, name)
        {
        }

        internal Expression ThisInstanceExpression
            => _thisInstanceExpression ??= new InstanceExpression(this, "this");

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