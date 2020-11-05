namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> for an
    /// <see cref="InterfaceExpression"/>.
    /// </summary>
    public interface IInterfacePropertyExpressionConfigurator
    {
        /// <summary>
        /// Add a getter to the <see cref="PropertyExpression"/>.
        /// </summary>
        void SetGetter();

        /// <summary>
        /// Add a setter to the <see cref="PropertyExpression"/>.
        /// </summary>
        void SetSetter();
    }
}