namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions;

    /// <summary>
    /// Provides options to configure a <see cref="MethodExpression"/>.
    /// </summary>
    public interface IMethodExpressionConfigurator
    {
        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MethodExpression"/>.
        /// </param>
        void SetSummary(CommentExpression summary);

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="MethodExpression"/>.
        /// </param>
        void SetVisibility(MemberVisibility visibility);

        /// <summary>
        /// Mark the <see cref="MethodExpression"/> as static.
        /// </summary>
        void SetStatic();

        /// <summary>
        /// Adds a <see cref="GenericParameterExpression"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <param name="configuration">The configuration to use for the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>The newly-created <see cref="GenericParameterExpression"/>.</returns>
        GenericParameterExpression AddGenericParameter(
            string name,
            Action<IGenericParameterExpressionConfigurator> configuration);

        /// <summary>
        /// Adds the given <paramref name="parameters"/> to the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="parameters">The ParameterExpression to add.</param>
        void AddParameters(params ParameterExpression[] parameters);

        /// <summary>
        /// Set the body of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="body">The Expression to use.</param>
        /// <param name="returnType">The return type to use for the method.</param>
        void SetBody(Expression body, Type returnType);
    }
}