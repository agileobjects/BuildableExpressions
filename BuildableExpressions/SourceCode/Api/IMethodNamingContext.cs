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
        /// <param name="summary">The summary documentation of the <see cref="MethodExpression"/>.</param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithSummary(string summary);

        /// <summary>
        /// Set the summary documentation of the <see cref="MethodExpression"/>.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="CommentExpression"/> containing summary documentation of the
        /// <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithSummary(CommentExpression summary);

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to give the <see cref="MethodExpression"/>.</param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator Named(string name);

        /// <summary>
        /// Gives the <see cref="MethodExpression"/> the given <paramref name="visibility"/>.
        /// </summary>
        /// <param name="visibility">
        /// The <see cref="MemberVisibility"/> to give the <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>This <see cref="IMethodExpressionConfigurator"/>, to support a fluent API.</returns>
        IMethodExpressionConfigurator WithVisibility(MemberVisibility visibility);
    }

    /// <summary>
    /// Provides information with which to name a <see cref="MethodExpression"/>.
    /// </summary>
    public interface IMethodNamingContext
    {
        /// <summary>
        /// Gets the return type of the LambdaExpression from which the method to which this
        /// <see cref="IMethodNamingContext"/> relates was created.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Gets a PascalCase, method-name-friendly translation of the return type of the
        /// LambdaExpression from which the method to which this <see cref="IMethodNamingContext"/>
        /// relates was created.
        /// </summary>
        string ReturnTypeName { get; }

        /// <summary>
        /// Gets the LambdaExpression from which the method to which this
        /// <see cref="IMethodNamingContext"/> relates was created.
        /// </summary>
        LambdaExpression MethodLambda { get; }

        /// <summary>
        /// Gets the index of the method in the set of generated class methods to which this
        /// <see cref="IMethodNamingContext"/> relates.
        /// </summary>
        int Index { get; }
    }
}