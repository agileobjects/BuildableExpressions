namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="MethodExpressionBase"/> ParameterExpression.
    /// </summary>
    public interface IParameterExpressionConfigurator : IAttributableExpressionConfigurator
    {
        /// <summary>
        /// Set the <see cref="MethodExpressionBase"/> ParameterExpression to an out parameter.
        /// </summary>
        void SetOut();
    }
}