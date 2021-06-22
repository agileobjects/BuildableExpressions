namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Logging;
    using ProjectManagement;

    internal class SourceCodeExpressionBuildContext : IExpressionBuildContext
    {
        private readonly ILogger _logger;
        private readonly SourceCodeExpressionBuildersFinder _buildersFinder;
        private readonly IProject _project;

        public SourceCodeExpressionBuildContext(
            ILogger logger,
            SourceCodeExpressionBuildersFinder buildersFinder,
            IProject project)
        {
            _logger = logger;
            _buildersFinder = buildersFinder;
            _project = project;
        }

        public IEnumerable<Assembly> ProjectAssemblies => _buildersFinder.ProjectAssemblies;
        
        public IEnumerable<Assembly> OutputAssemblies => _buildersFinder.OutputAssemblies;

        public string RootNamespace => _project.RootNamespace;

        public void Log(string message) => _logger.Info(message);

        public void Log(Exception exception) => _logger.Error(exception);
    }
}