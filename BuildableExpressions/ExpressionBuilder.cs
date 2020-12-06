﻿namespace DefaultNamespace
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
        /// <returns><see cref="SourceCodeExpression"/>s to compile to source code files.</returns>
        public IEnumerable<SourceCodeExpression> Build()
        {
            // Replace this code with your own, building SourceCodeExpression(s)
            // to be compiled to a source code file:
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(typeof(ExpressionBuilder).Name + "OutputClass", cls =>
                {
                    var doNothing = Expression.Empty();

                    cls.AddMethod("DoNothing", doNothing);
                });
            });

            yield return sourceCode;
        }
    }
}
