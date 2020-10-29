namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using Generics;

    /// <summary>
    /// Provides options to configure a <see cref="GenericParameterExpression"/>.
    /// </summary>
    public interface IGenericParameterExpressionConfigurator
    {
        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to have a struct constraint.
        /// </summary>
        void AddStructConstraint();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to have a class constraint.
        /// </summary>
        void AddClassConstraint();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to have a new() constraint.
        /// </summary>
        void AddNewableConstraint();

        /// <summary>
        /// Set the <see cref="GenericParameterExpression"/> to be constrained to the given
        /// <paramref name="types"/>.
        /// </summary>
        /// <param name="types">The Type(s) to which to constrain the <see cref="GenericParameterExpression"/>.</param>
        void AddTypeConstraints(params Type[] types);
    }
}