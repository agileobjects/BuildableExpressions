namespace AgileObjects.BuildableExpressions.Generator.SourceCode
{
    using System;
    using Configuration;
    using InputOutput;
    using Logging;
    using ProjectManagement;

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
                _logger.Info($"project '{projectName}' starting...");

                var builders = _buildersFinder.Find(config);

                if (builders.Count == 0)
                {
                    goto Complete;
                }

                _logger.Info($"{builders.Count} builder(s) found...");
                var outputProject = _projectFactory.GetOutputProjectOrThrow(config);

                var sourceCodeExpressions = builders.ToSourceCodeExpressions(_buildersFinder);
                _logger.Info($"{sourceCodeExpressions.Count} Expression(s) built...");

                var writtenFiles = _outputWriter.Write(config, sourceCodeExpressions);

                outputProject.Add(writtenFiles);
                
                _logger.Info(
                    $"{writtenFiles.Count} file(s) written to " +
                    $"project '{config.GetOutputProjectNameWithoutExtension()}'");
                
                result.BuiltExpressionsCount = writtenFiles.Count;

            Complete:
                _logger.Info($"project '{projectName}' complete");
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
