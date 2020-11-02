namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="PropertyExpression"/> setter accessor.
    /// </summary>
    public interface IPropertySetterConfigurator
    {
        /// <summary>
        /// Gives the <see cref="PropertyExpression"/> setter the given
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
        /// <param name="bodyFactory">
        /// A Func which takes a ParameterExpression representing the property setter's 'value'
        /// keyword, and returns the Expression to use.
        /// </param>
        void SetBody(Func<ParameterExpression, Expression> bodyFactory);
    }
}