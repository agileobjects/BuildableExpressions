namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to configure a <see cref="GenericParameterExpression"/>.
    /// </summary>
    public interface IGenericParameterExpressionConfigurator
    {
        /// <summary>
        /// Set the name of the <see cref="GenericParameterExpression"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator Named(string name);

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to have a struct constraint.
        /// </summary>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator WithStructConstraint();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to have a class constraint.
        /// </summary>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator WithClassConstraint();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to have a new() constraint.
        /// </summary>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator WithNewableConstraint();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <typeparamref name="T"/> Type.
        /// </summary>
        /// <typeparam name="T">The Type to which to constrain the <see cref="GenericParameterExpression"/>.</typeparam>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator WithTypeConstraint<T>();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The Type to which to constrain the <see cref="GenericParameterExpression"/>.</param>
        /// <returns>This <see cref="IGenericParameterExpressionConfigurator"/>, to support a fluent API.</returns>
        IGenericParameterExpressionConfigurator WithTypeConstraint(Type type);
    }
}