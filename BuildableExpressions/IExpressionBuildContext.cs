namespace AgileObjects.BuildableExpressions
{
    using System.Collections.Generic;
    using System.Reflection;
    using SourceCode;

    /// <summary>
    /// Implementing classes will describe the context in which <see cref="SourceCodeExpression"/>s
    /// are being built.
    /// </summary>
    public interface IExpressionBuildContext
    {
        /// <summary>
        /// Gets the loaded project Assemblies from which <see cref="ISourceCodeExpressionBuilder"/>s
        /// have been retrieved.
        /// </summary>
        IEnumerable<Assembly> ProjectAssemblies { get; }
    }
}