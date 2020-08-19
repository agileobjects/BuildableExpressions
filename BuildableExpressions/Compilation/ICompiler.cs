namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Generic;

    internal interface ICompiler
    {
        CompilationResult Compile(
            string expressionBuilderSource,
            ICollection<Type> referenceAssemblyTypes);
    }
}
