namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Configuration;
    using Logging;
    using ProjectManagement;

    internal class SourceCodeExpressionBuildContext : IExpressionBuildContext
    {
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly SourceCodeExpressionBuildersFinder _buildersFinder;
        private readonly IProject _outputProject;

        public SourceCodeExpressionBuildContext(
            ILogger logger,
            IConfig config,
            SourceCodeExpressionBuildersFinder buildersFinder,
            IProject outputProject)
        {
            _logger = logger;
            _config = config;
            _buildersFinder = buildersFinder;
            _outputProject = outputProject;
        }

        public string InputProjectOutputPath => _config.GetInputPath();
        
        public IEnumerable<Assembly> ProjectAssemblies => _buildersFinder.ProjectAssemblies;
        
        public IEnumerable<Assembly> OutputAssemblies => _buildersFinder.OutputAssemblies;

        public string RootNamespace => _outputProject.RootNamespace;

        public void Log(string message) => _logger.Info(message);

        public void Log(Exception exception) => _logger.Error(exception);
    }
}