// ReSharper disable once CheckNamespace
namespace BuildXpr
{
    using AgileObjects.BuildableExpressions.Generator.Compilation;
    using AgileObjects.BuildableExpressions.Generator.Configuration;
    using AgileObjects.BuildableExpressions.Generator.InputOutput;
    using AgileObjects.BuildableExpressions.Generator.Logging;
    using AgileObjects.BuildableExpressions.Generator.ProjectManagement;
    using AgileObjects.BuildableExpressions.Generator.SourceCode;
    using AgileObjects.BuildableExpressions.SourceCode;
    using Microsoft.Build.Framework;
    using MsBuildTask = Microsoft.Build.Utilities.Task;

    /// <summary>
    /// An MSBuild Task to generate source code files from <see cref="SourceCodeExpression"/>s.
    /// </summary>
    public class BuildExpressionsTask : MsBuildTask, IConfig
    {
        /// <summary>
        /// Gets or sets the full path of the solution providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string SolutionPath { get; set; }

        /// <summary>
        /// Gets or sets the full path of the project providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the root namespace of the project providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Gets or sets the relative path to which build output is written.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of <see cref="SourceCodeExpression"/>s built
        /// into output files by the execution of this <see cref="BuildExpressionsTask"/>.
        /// </summary>
        [Output]
        public int BuiltExpressionsCount { get; set; }

        /// <summary>
        /// Generates source code files from a set of <see cref="SourceCodeExpression"/>s.
        /// </summary>
        public override bool Execute()
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            var logger = new MsBuildTaskLogger(Log);
            var fileManager = SystemIoFileManager.Instance;
            var assemblyResolver = new AssemblyResolver(logger, fileManager);

            var generator = new SourceCodeGenerator(
                logger,
                new ProjectFactory(logger, fileManager),
                new ExpressionBuildersFinder(logger, assemblyResolver),
                new OutputWriter(logger, fileManager));

            var result = generator.Execute(this);

            BuiltExpressionsCount = result.BuiltExpressionsCount;
            return result.Success;
        }
    }
}
