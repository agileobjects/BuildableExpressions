namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="FieldExpression"/>.
    /// </summary>
    public interface IFieldExpressionConfigurator : IMemberExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="FieldExpression"/> as static.
        /// </summary>
        void SetStatic();

        /// <summary>
        /// Mark the class <see cref="FieldExpression"/> as readonly.
        /// </summary>
        void SetReadonly();
    }
}