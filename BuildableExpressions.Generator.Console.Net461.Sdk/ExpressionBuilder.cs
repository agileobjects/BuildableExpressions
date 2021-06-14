namespace DefaultNamespace
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AgileObjects.BuildableExpressions;
    using AgileObjects.BuildableExpressions.SourceCode;

    /// <summary>
    /// Supplies a set of input <see cref="SourceCodeExpression"/>s to compile to source code when
    /// this project is built.
    /// </summary>
    public class ExpressionBuilder : ISourceCodeExpressionBuilder
    {
        /// <summary>
        /// Builds one or more <see cref="SourceCodeExpression"/>s to compile to source code files
        /// when this project is built.
        /// </summary>
        /// <param name="context">
        /// An <see cref="IExpressionBuildContext"/> describing the context in which the
        /// <see cref="SourceCodeExpression"/>s are being built.
        /// </param>
        /// <returns><see cref="SourceCodeExpression"/>s to compile to source code files.</returns>
        public IEnumerable<SourceCodeExpression> Build(IExpressionBuildContext context)
        {
            // Replace this code with your own, building SourceCodeExpression(s)
            // to be compiled to one or more source code files:
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(nameof(ExpressionBuilder) + "OutputClass", cls =>
                {
                    var doNothing = Expression.Empty();

                    cls.AddMethod("DoNothingNet461Sdk", doNothing);
                });
            });

            yield return sourceCode;
        }
    }
}
