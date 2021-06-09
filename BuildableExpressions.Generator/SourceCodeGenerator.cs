namespace AgileObjects.BuildableExpressions.Generator
{
    using System;
    using Configuration;
    using InputOutput;
    using Logging;
    using ProjectManagement;
    using SourceCode;

    internal class SourceCodeGenerator
    {
        private readonly ILogger _logger;
        private readonly IProjectFactory _projectFactory;
        private readonly ExpressionBuildersFinder _buildersFinder;
        private readonly OutputWriter _outputWriter;

        public SourceCodeGenerator(
            ILogger logger,
            IProjectFactory projectFactory,
            ExpressionBuildersFinder buildersFinder,
            OutputWriter outputWriter)
        {
            _logger = logger;
            _projectFactory = projectFactory;
            _buildersFinder = buildersFinder;
            _outputWriter = outputWriter;
        }

        public SourceCodeGenerationResult Execute(IConfig config)
        {
            var result = new SourceCodeGenerationResult();

            try
            {
                var projectName = config.GetProjectNameWithoutExtension();
                _logger.Info($"Source Code Expressions for project '{projectName}': transpiling...");

                var builders = _buildersFinder.Find(config);

                if (builders.Count == 0)
                {
                    goto Complete;
                }

                _logger.Info($"Source Code Expression Builders: {builders.Count} found...");
                var project = _projectFactory.GetProjectOrThrow(config);

                var sourceCodeExpressions = builders.ToSourceCodeExpressions();
                _logger.Info($"Source Code Expressions: {sourceCodeExpressions.Count} built...");

                var writtenFiles = _outputWriter.Write(config, sourceCodeExpressions);

                project.Add(writtenFiles);
                _logger.Info($"Source Code Expression files: {writtenFiles.Count} written");
                result.BuiltExpressionsCount = writtenFiles.Count;

            Complete:
                _logger.Info($"Source Code Expressions for project '{projectName}': transpiling complete");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            
            return result;
        }

        public class SourceCodeGenerationResult
        {
            public bool Success { get; set; }
            
            public int BuiltExpressionsCount { get; set; }
        }
    }
}
