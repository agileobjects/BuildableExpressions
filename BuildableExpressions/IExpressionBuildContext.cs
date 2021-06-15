namespace AgileObjects.BuildableExpressions
{
    using System;
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

        /// <summary>
        /// Log the given <paramref name="message"/> to the build output.
        /// </summary>
        /// <param name="message">The message to log to the build output.</param>
        void Log(string message);

        /// <summary>
        /// Log the given <paramref name="exception"/> to the build output.
        /// </summary>
        /// <param name="exception">The Exception to log to the build output.</param>
        void Log(Exception exception);
    }
}