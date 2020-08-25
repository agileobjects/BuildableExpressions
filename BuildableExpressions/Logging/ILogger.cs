namespace AgileObjects.BuildableExpressions.Logging
{
    using System;
    using MsBuildTask = Microsoft.Build.Utilities.Task;

    internal interface ILogger
    {
        ILogger WithTask(MsBuildTask task);

        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void Error(Exception ex);
    }
}
