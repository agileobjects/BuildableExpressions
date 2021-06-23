#if NET5_0
namespace AgileObjects.BuildableExpressions.Generator
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Configuration;
    using Logging;
    using SourceCode;

    /// <summary>
    /// Entry point for the .NET Core App Expression Builder. The .NET Core App target is used to
    /// generate source code for .NET Core / .NET 5 apps via Visual Studio MSBuild, as Visual Studio
    /// MSBuild runs in the .NET Framework, and is not capable of executing
    /// ISourceCodeExpressionBuilder.Build() methods compiled for .NET Core.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Generates source code from Source Code Expressions using information in the given
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// Arguments detailing the source code generation environment:
        /// [0] - The path to the current Solution
        /// [1] - The path to the input Project
        /// [2] - The path to the output Project
        /// [3] - The currently-building Target Framework
        /// [4] - The relative path to the input project's output directory
        /// [5] - A boolean string indicating whether to launch the debugger.
        /// </param>
        public static void Main(string[] args)
        {
            if (bool.Parse(args[5]))
            {
                Debugger.Launch();
            }

            var logger = new StandardOutLogger();

            var config = new Config
            {
                SolutionPath = args[0],
                InputProjectPath = args[1],
                OutputProjectPath = args[2],
                TargetFramework = args[3],
                InputDirectory = args[4]
            };

            var result = SourceCodeGenerator.Execute(logger, config);
            logger.Info(result.Success + ":" + result.BuiltExpressionsCount);
        }

        private class StandardOutLogger : ILogger
        {
            public void Info(string message)
            {
                using var writer = new StreamWriter(Console.OpenStandardOutput());
                writer.WriteLine(message);
            }

            public void Warning(string message) => Info(message);

            public void Error(Exception ex) => Console.Error.WriteLine(ex.ToString());
        }

        private class Config : IConfig
        {
            public string SolutionPath { get; set; }

            public string InputProjectPath { get; set; }

            public string OutputProjectPath { get; set; }

            public string TargetFramework { get; set; }

            public string InputDirectory { get; set; }
        }
    }
}
#endif