namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Linq.Expressions;

    /// <summary>
    /// Provides options to configure a <see cref="FieldExpression"/>.
    /// </summary>
    public interface IFieldExpressionConfigurator : IMemberExpressionConfigurator
    {
        /// <summary>
        /// Mark the <see cref="FieldExpression"/> as static.
        /// </summary>
        void SetStatic();

        /// <summary>
        /// Mark the class <see cref="FieldExpression"/> as constant.
        /// </summary>
        void SetConstant();

        /// <summary>
        /// Mark the class <see cref="FieldExpression"/> as readonly.
        /// </summary>
        void SetReadonly();

        /// <summary>
        /// Set the <see cref="FieldExpression"/>'s initial value to the given
        /// <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to set.</typeparam>
        /// <param name="value">The value to which to initialise the <see cref="FieldExpression"/>.</param>
        void SetInitialValue<TValue>(TValue value);

        /// <summary>
        /// Set the <see cref="FieldExpression"/>'s initial value to the given
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// An Expression representing the value to which to initialise the
        /// <see cref="FieldExpression"/>.
        /// </param>
        void SetInitialValue(Expression value);
    }
}