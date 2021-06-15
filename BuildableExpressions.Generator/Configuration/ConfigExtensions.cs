namespace AgileObjects.BuildableExpressions.Generator.Configuration
{
    using System.IO;
    using ProjectManagement;

    internal static class ConfigExtensions
    {
        public static string GetSolutionName(this IConfig config)
            => Path.GetFileName(config.SolutionPath);

        public static string GetProjectNameWithoutExtension(this IConfig config)
            => GetProjectNameWithoutExtension(config.InputProjectPath);

        public static string GetOutputProjectNameWithoutExtension(this IConfig config)
            => GetProjectNameWithoutExtension(config.OutputProjectPath);

        private static string GetProjectNameWithoutExtension(string projectPath)
            => Path.GetFileNameWithoutExtension(Path.GetFileName(projectPath));

        public static string GetOutputRoot(this IConfig config)
            => Path.GetDirectoryName(config.OutputProjectPath);

        public static string GetInputPath(this IConfig config)
            => Path.Combine(config.GetInputRoot(), config.InputDirectory);

        public static string GetInputRoot(this IConfig config)
            => Path.GetDirectoryName(config.InputProjectPath);
    }
}