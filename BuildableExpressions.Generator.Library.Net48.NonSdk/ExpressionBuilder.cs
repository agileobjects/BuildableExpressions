namespace DefaultNamespace
{
    using System.Collections.Generic;
    using AgileObjects.BuildableExpressions;
    using AgileObjects.BuildableExpressions.SourceCode;

    /// <summary>
    /// Supplies an input <see cref="SourceCodeExpression"/> to compile to source code when this
    /// project is built.
    /// </summary>
    public class ExpressionBuilder : ISourceCodeExpressionBuilder
    {
        /// <summary>
        /// Builds a <see cref="SourceCodeExpression"/> to compile to a source code file when this
        /// project is built.
        /// </summary>
        /// <param name="context">
        /// An <see cref="IExpressionBuildContext"/> describing the context in which the
        /// <see cref="SourceCodeExpression"/>s are being built.
        /// </param>
        /// <returns>A <see cref="SourceCodeExpression"/> to compile to a source code file.</returns>
        public IEnumerable<SourceCodeExpression> Build(IExpressionBuildContext context)
        {
            var sourceCode = BuildableExpression.SourceCode(@"
namespace AgileObjects.BuildableExpressions.Generator.Console
{
    public struct ExpressionBuilderOutputStruct
    {
        public void DoNothingNet48()
        {
        }
    }
}
");
            yield return sourceCode;
        }
    }
}
