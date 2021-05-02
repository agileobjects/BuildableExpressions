namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System.Collections.ObjectModel;
    using ReadableExpressions.Translations.Reflection;

    internal interface IClosableTypeExpression : IType
    {
        ReadOnlyCollection<GenericParameterExpression> GenericParameters { get; }

        IClosableTypeExpression Close(
            GenericParameterExpression genericParameter,
            TypeExpression closedTypeExpression);
    }
}
