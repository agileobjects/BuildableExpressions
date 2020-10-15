namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;

    /// <summary>
    /// Provides configuration options to control aspects of source-code generation.
    /// </summary>
    public interface ISourceCodeTranslationSettings
    {
        /// <summary>
        /// Name generated classes using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated class. The parent
        /// <see cref="SourceCodeExpression"/> and a <see cref="IClassNamingContext"/> are supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameClassesUsing(
            Func<SourceCodeExpression, IClassNamingContext, string> nameFactory);

        /// <summary>
        /// Name generated classes using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated class. An
        /// <see cref="IClassNamingContext"/> is supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameClassesUsing(Func<IClassNamingContext, string> nameFactory);

        /// <summary>
        /// Name generated methods using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated method. The parent
        /// <see cref="SourceCodeExpression"/>, <see cref="ClassExpression"/> and an
        /// <see cref="IMethodNamingContext"/> are supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameMethodsUsing(
            Func<SourceCodeExpression, ClassExpression, IMethodNamingContext, string> nameFactory);

        /// <summary>
        /// Name generated methods using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated method. The parent
        /// <see cref="ClassExpression"/> and an <see cref="IMethodNamingContext"/> are supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameMethodsUsing(Func<ClassExpression, IMethodNamingContext, string> nameFactory);

        /// <summary>
        /// Name generated methods using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated method. An
        /// <see cref="IMethodNamingContext"/> is supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameMethodsUsing(Func<IMethodNamingContext, string> nameFactory);

        /// <summary>
        /// Name <see cref="GenericParameterExpression"/>s using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a <see cref="GenericParameterExpression"/>.
        /// The parent <see cref="MethodExpression"/> and an <see cref="IGenericParameterNamingContext"/>
        /// are supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameGenericParametersUsing(
            Func<MethodExpression, IGenericParameterNamingContext, string> nameFactory);
    }
}