namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Provides options to configure a <see cref="ConcreteTypeExpression"/>.
    /// </summary>
    public interface IConcreteTypeExpressionConfigurator :
        ITypeExpressionConfigurator,
        IConstructorConfigurator
    {
        /// <summary>
        /// Gets an Expression which refers to the instance of the type being created in the current
        /// scope. This property can be used to access the 'this' keyword in a method.
        /// </summary>
        Expression ThisInstanceExpression { get; }

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="FieldExpression"/> to the
        /// <see cref="ConcreteTypeExpression"/>, with the given <paramref name="name"/> and
        /// <typeparamref name="TField"/> type.
        /// </summary>
        /// <typeparam name="TField">The type of the <see cref="FieldExpression"/>.</typeparam>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        FieldExpression AddField<TField>(string name);

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="FieldExpression"/> to the
        /// <see cref="ConcreteTypeExpression"/>, with the given <paramref name="name"/>,
        /// <typeparamref name="TField"/> type and <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TField">The type of the <see cref="FieldExpression"/>.</typeparam>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        FieldExpression AddField<TField>(
            string name,
            Action<IFieldExpressionConfigurator> configuration);

        /// <summary>
        /// Add a public, instance-scoped, get-set <see cref="FieldExpression"/> to the
        /// <see cref="ConcreteTypeExpression"/>, with the given <paramref name="name"/> and
        /// <paramref name="type"/> .
        /// </summary>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <param name="type">The Type of the <see cref="FieldExpression"/>.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        FieldExpression AddField(string name, Type type);

        /// <summary>
        /// Add a <see cref="FieldExpression"/> to the <see cref="ConcreteTypeExpression"/>, with
        /// the given <paramref name="name"/>, <paramref name="type"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <param name="type">The Type of the <see cref="FieldExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        FieldExpression AddField(
            string name,
            Type type,
            Action<IFieldExpressionConfigurator> configuration);

        /// <summary>
        /// Add a <see cref="FieldExpression"/> to the <see cref="ConcreteTypeExpression"/>, with
        /// the given <paramref name="name"/>, <paramref name="type"/> and
        /// <paramref name="configuration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="FieldExpression"/>.</param>
        /// <param name="type">The <see cref="IType"/> of the <see cref="FieldExpression"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns>The newly-created <see cref="FieldExpression"/>.</returns>
        FieldExpression AddField(
            string name,
            IType type,
            Action<IFieldExpressionConfigurator> configuration);
    }
}