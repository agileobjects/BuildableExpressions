namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/>.
    /// </summary>
    public interface IPropertyExpressionConfigurator
    {
        /// <summary>
        /// Gives the <see cref="PropertyExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="PropertyExpression"/>.
        /// </param>
        void SetVisibility(MemberVisibility visibility);
    }
}