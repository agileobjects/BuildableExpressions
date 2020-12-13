namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Linq.Expressions;
    using Generics;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Provides options to configure a <see cref="ConcreteTypeExpression"/>.
    /// </summary>
    public interface IConcreteTypeExpressionConfigurator : 
        ITypeableTypeExpressionConfigurator,
        IConstructorConfigurator
    {
        /// <summary>
        /// Gets an Expression to use to refer to the instance of the type being created in the
        /// current scope. Use this property to access the 'this' keyword in a class or struct method.
        /// </summary>
        Expression ThisInstanceExpression { get; }

        /// <summary>
        /// Adds the given open generic <paramref name="parameter"/> to the
        /// <see cref="ConcreteTypeExpression"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="GenericParameterExpression"/> to add.</param>
        void AddGenericParameter(GenericParameterExpression parameter);

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