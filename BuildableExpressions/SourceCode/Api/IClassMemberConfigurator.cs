﻿namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides options to add a <see cref="PropertyExpression"/> or <see cref="MethodExpression"/>
    /// to a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassMemberConfigurator
    {
        /// <summary>
        /// Add a <see cref="PropertyExpression"/> to the <see cref="ClassExpression"/>, with
        /// the given <paramref name="name"/>, <paramref name="type"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="PropertyExpression"/>.</param>
        /// <param name="type">The type of the <see cref="PropertyExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="PropertyExpression"/>.</returns>
        PropertyExpression AddProperty(
            string name,
            Type type,
            Action<IClassPropertyExpressionConfigurator> configuration);

        /// <summary>
        /// Add a <see cref="MethodExpression"/> to the <see cref="ClassExpression"/>, with
        /// the given <paramref name="name"/> and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MethodExpression"/>.</param>
        /// <param name="configuration">
        /// The configuration with which to configure the new <see cref="MethodExpression"/>.
        /// </param>
        /// <returns>The newly-created <see cref="MethodExpression"/>.</returns>
        MethodExpression AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration);
    }
}