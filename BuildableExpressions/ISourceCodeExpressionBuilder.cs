namespace AgileObjects.BuildableExpressions
{
    using System.Collections.Generic;
    using SourceCode;

    /// <summary>
    /// Implementing classes will build one or more <see cref="SourceCodeExpression"/>s to be
    /// translated into source code files during compilation.
    /// </summary>
    public interface ISourceCodeExpressionBuilder
    {
        /// <summary>
        /// Builds the <see cref="SourceCodeExpression"/>s which should be translated into source code
        /// files during compilation.
        /// </summary>
        /// <returns>One or more <see cref="SourceCodeExpression"/>s.</returns>
        IEnumerable<SourceCodeExpression> Build();
    }
}
