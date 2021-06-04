namespace AgileObjects.BuildableExpressions.Generator.Logging
{
    using System;

    internal interface ILogger
    {
        void Info(string message);
        
        void Warning(string message);

        void Error(string message);

        void Error(Exception ex);
    }
}
