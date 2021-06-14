// ReSharper disable once CheckNamespace
namespace XprGenerator
{
    using System.Diagnostics;
    using AgileObjects.BuildableExpressions.Generator.Compilation;
    using AgileObjects.BuildableExpressions.Generator.Configuration;
    using AgileObjects.BuildableExpressions.Generator.Extensions;
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
    public class XprGenerateTask : MsBuildTask, IConfig
    {
        private readonly MsBuildTaskLogger _logger;
        private readonly IFileManager _fileManager;
        private readonly AssemblyResolver _assemblyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="XprGenerateTask"/> class.
        /// </summary>
        public XprGenerateTask()
        {
            _logger = new MsBuildTaskLogger(Log);
            _fileManager = SystemIoFileManager.Instance;
            _assemblyResolver = new AssemblyResolver(_logger, _fileManager);
        }

        /// <summary>
        /// Gets or sets the full path of the solution providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string SolutionPath { get; set; }

        /// <summary>
        /// Gets or sets the full path of the project providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string InputProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the full path of the project to which the generated 
        /// <see cref="SourceCodeExpression"/> classes should be added.
        /// </summary>
        public string OutputProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the root namespace of the project providing the 
        /// <see cref="SourceCodeExpression"/>(s) to build.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Gets or sets the target framework against which <see cref="SourceCodeExpression"/>(s)
        /// are being built.
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// Gets or sets the relative path to which the input project's build output is written.
        /// </summary>
        public string InputDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the debugger should be launched during the
        /// task execution.
        /// </summary>
        public string Debug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of <see cref="SourceCodeExpression"/>s built
        /// into output files by the execution of this <see cref="XprGenerateTask"/>.
        /// </summary>
        [Output]
        public int BuiltExpressionsCount { get; set; }

        /// <summary>
        /// Generates source code files from a set of <see cref="SourceCodeExpression"/>s.
        /// </summary>
        public override bool Execute()
        {
            if (LaunchDebugger)
            {
                Debugger.Launch();
            }

            _assemblyResolver.Init(this);

            if (TryBuildExpressions(out var builtExpressionsCount))
            {
                BuiltExpressionsCount = builtExpressionsCount;
                return true;
            }

            return false;
        }

        private bool TryBuildExpressions(out int builtExpressionsCount)
        {
            var generator = new SourceCodeGenerator(
                _logger,
                new ProjectFactory(_fileManager),
                new ExpressionBuildersFinder(_logger, _assemblyResolver),
                new OutputWriter(_logger, _fileManager));

            var result = generator.Execute(this);

            builtExpressionsCount = result.BuiltExpressionsCount;
            return result.Success;
        }

        private bool LaunchDebugger => bool.TryParse(Debug, out var value) && value;

        string IConfig.TargetFramework => GetTargetFramework();

        private string GetTargetFramework()
        {
            const string net40Wildcard = "net4*";

            if (string.IsNullOrEmpty(TargetFramework))
            {
                return net40Wildcard;
            }

            return TargetFramework.StartsWithIgnoreCase("net4") ? net40Wildcard : "netstandard2.0";
        }
    }
}
