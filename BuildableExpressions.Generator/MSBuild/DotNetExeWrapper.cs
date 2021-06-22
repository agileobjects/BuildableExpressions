#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.MSBuild
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Configuration;
    using Logging;
    using static System.Environment;
    using static System.StringSplitOptions;

    internal static class DotNetExeWrapper
    {
        private static readonly Regex _resultMatcher = 
            new(@"^(?:true|false):(?:\d)", RegexOptions.IgnoreCase);

        public static bool RunTask(
            ILogger logger,
            IConfig config,
            bool debug,
            out int builtExpressionsCount)
        {
            var path = typeof(DotNetExeWrapper)
                .Assembly.Location
                .Replace("net461", "net5.0")
                .Replace(".exe", ".dll");

            var args = string.Join(" ", new[]
            {
                config.SolutionPath,
                config.InputProjectPath,
                config.OutputProjectPath,
                config.TargetFramework,
                config.InputDirectory,
                debug.ToString()
            }.Select(arg => $"\"{arg}\""));

            var dotnetProcessStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{path}\" {args}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var dotnetProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = dotnetProcessStartInfo
            };

            var result = false;
            var builtExprCount = 0;

            dotnetProcess.OutputDataReceived += (_, ev) =>
            {
                if (ev.Data == null)
                {
                    return;
                }

                if (_resultMatcher.IsMatch(ev.Data))
                {
                    var resultInfo = ev.Data.Split(':');
                    result = bool.Parse(resultInfo[0]);
                    builtExprCount = int.Parse(resultInfo[1]);
                    return;
                }

                foreach (var message in SplitToLines(ev.Data))
                {
                    logger.Info(message);
                }
            };

            dotnetProcess.ErrorDataReceived += (_, err) =>
            {
                if (err.Data != null)
                {
                    logger.Error(new Exception(err.Data));
                }
            };

            dotnetProcess.Start();
            dotnetProcess.BeginOutputReadLine();
            dotnetProcess.BeginErrorReadLine();

            dotnetProcess.WaitForExit();
            
            builtExpressionsCount = builtExprCount;
            return result && dotnetProcess.ExitCode == 0;
        }

        private static IEnumerable<string> SplitToLines(string value)
            => value.Split(new[] { NewLine }, RemoveEmptyEntries);
    }
}
#endif