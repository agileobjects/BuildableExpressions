namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="PropertyOrFieldExpression"/>.
    /// </summary>
    public interface IPropertyExpressionConfigurator
    {
        /// <summary>
        /// Gives the <see cref="PropertyOrFieldExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="PropertyOrFieldExpression"/>.
        /// </param>
        void SetVisibility(MemberVisibility visibility);
    }
}