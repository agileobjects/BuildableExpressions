namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    using System.IO;

    internal static class ConfigExtensions
    {
        public static string GetSolutionName(this IConfig config)
            => Path.GetFileName(config.SolutionPath);

        public static string GetContentRoot(this IConfig config)
            => Path.GetDirectoryName(config.ProjectPath);

        public static string GetOutputPath(this IConfig config)
            => Path.Combine(config.GetContentRoot(), config.OutputDirectory);
    }
}