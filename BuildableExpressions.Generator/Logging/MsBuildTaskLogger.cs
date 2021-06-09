namespace AgileObjects.BuildableExpressions.Generator.Logging
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    internal class MsBuildTaskLogger : ILogger
    {
        private readonly TaskLoggingHelper _taskLog;

        public MsBuildTaskLogger(TaskLoggingHelper taskLog)
        {
            _taskLog = taskLog;
        }

        public void Info(string message)
            => _taskLog.LogMessage(MessageImportance.High, message);

        public void Warning(string message)
            => _taskLog.LogMessage(MessageImportance.High, message);

        public void Error(Exception ex)
            => _taskLog.LogErrorFromException(ex);
    }
}
