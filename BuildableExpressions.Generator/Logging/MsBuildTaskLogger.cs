#if FEATURE_MSBUILD
namespace AgileObjects.BuildableExpressions.Generator.Logging
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using static System.StringComparison;

    internal class MsBuildTaskLogger : ILogger
    {
        private readonly TaskLoggingHelper _taskLog;
        private readonly string _prefix;

        public MsBuildTaskLogger(TaskLoggingHelper taskLog, string prefix)
        {
            _taskLog = taskLog;

            prefix = prefix.TrimEnd();

            if (!prefix.EndsWith(":", Ordinal))
            {
                prefix += ":";
            }
            
            _prefix = prefix + " ";
        }

        public void Info(string message)
            => _taskLog.LogMessage(MessageImportance.High, _prefix + message);

        public void Warning(string message)
            => _taskLog.LogMessage(MessageImportance.High, _prefix + message);

        public void Error(Exception ex)
            => _taskLog.LogErrorFromException(ex);
    }
}
#endif