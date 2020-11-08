namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    /// <summary>
    /// Provides options to configure an <see cref="EnumExpression"/>.
    /// </summary>
    public interface IEnumExpressionConfigurator : ITypeExpressionConfigurator
    {
        /// <summary>
        /// Add members to the <see cref="EnumExpression"/> with the given
        /// <paramref name="memberNames"/> and auto-generated values.
        /// </summary>
        /// <param name="memberNames">The names of the enum member.</param>
        void AddMembers(params string[] memberNames);

        /// <summary>
        /// Add a member to the <see cref="EnumExpression"/> with the given <paramref name="name"/>
        /// and <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the enum member.</param>
        /// <param name="value">The value of the enum member.</param>
        void AddMember(string name, int value);
    }
}