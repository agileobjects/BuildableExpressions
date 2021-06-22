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
        /// Gets the project Assemblies from which <see cref="ISourceCodeExpressionBuilder"/>s have
        /// been retrieved.
        /// </summary>
        IEnumerable<Assembly> ProjectAssemblies { get; }

        /// <summary>
        /// Gets all Assemblies output by the build of the current project.
        /// </summary>
        IEnumerable<Assembly> OutputAssemblies { get; }

        /// <summary>
        /// Gets the root namespace to which generated source code will be added. Defaults to the
        /// name of the target project, or the value of its &lt;RootNamespace&gt; property if
        /// present.
        /// </summary>
        string RootNamespace { get; }

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