namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> getter accessor.
    /// </summary>
    public interface IPropertyGetterConfigurator : IAttributableExpressionConfigurator
    {
        /// <summary>
        /// Gives the <see cref="PropertyExpression"/> getter the given
        /// <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="PropertyExpression"/>
        /// accessor.
        /// </param>
        void SetVisibility(MemberVisibility visibility);

        /// <summary>
        /// Set the body of the <see cref="PropertyExpression"/> getter.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        void SetBody(Expression body);
    }
}