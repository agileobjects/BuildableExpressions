namespace DefaultNamespace
{
    using System;
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
        /// <returns><see cref="SourceCodeExpression"/>s to compile to source code files.</returns>
        public IEnumerable<SourceCodeExpression> Build()
        {
            // Replace this code with your own, building a SourceCodeExpression
            // to be compiled to a source code file:
            var doNothing = Expression.Lambda<Action>(Expression.Default(typeof(void)));

            yield return SourceCodeFactory
                .SourceCode(sc => sc
                    .WithNamespaceOf<ExpressionBuilder>()
                    .WithClass(typeof(ExpressionBuilder).Name + "OutputClass", cls => cls
                        .WithMethod("DoNothing", doNothing)));
        }
    }
}
