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
        /// The factory from which to obtain the name of a generated class. The
        /// <see cref="IClassNamingContext"/> is supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameClassesUsing(Func<IClassNamingContext, string> nameFactory);

        /// <summary>
        /// Name generated methods using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated method. The parent
        /// <see cref="SourceCodeExpression"/>, <see cref="ClassExpression"/> and a
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
        /// <see cref="ClassExpression"/> and method <see cref="IMethodNamingContext"/> are
        /// supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameMethodsUsing(Func<ClassExpression, IMethodNamingContext, string> nameFactory);

        /// <summary>
        /// Name generated methods using the given <paramref name="nameFactory"/>.
        /// </summary>
        /// <param name="nameFactory">
        /// The factory from which to obtain the name of a generated method. The method 
        /// <see cref="IMethodNamingContext"/> is supplied.
        /// </param>
        /// <returns>These <see cref="ISourceCodeTranslationSettings"/>, to support a fluent API.</returns>
        ISourceCodeTranslationSettings NameMethodsUsing(Func<IMethodNamingContext, string> nameFactory);
    }
}