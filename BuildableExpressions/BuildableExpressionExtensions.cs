namespace AgileObjects.BuildableExpressions
{
    using System.IO;

    /// <summary>
    /// Provides static values and helper methods.
    /// </summary>
    public static class BuildableExpressionExtensions
    {
        private static readonly string _tempOutputDirectory = Path.Combine(
            Path.GetDirectoryName(Path.GetTempFileName())!,
            nameof(BuildableExpressions));

        /// <summary>
        /// Gets a path to which a temporary file can be written, based on the given
        /// <paramref name="fileName"/> and with the given <paramref name="extension"/>.
        /// </summary>
        /// <param name="fileName">
        /// The name of the temporary file to write. A random string will be appended to this file
        /// name to yield a unique temporary file name.
        /// </param>
        /// <param name="extension">The file extension to use.</param>
        /// <returns>A path to which a temporary file can be written.</returns>
        public static string GetTempCopyFilePath(string fileName, string extension)
        {
            var tempFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            var tempCopyFileName = fileName + "_" + tempFileName + extension;
            var tempCopyFilePath = Path.Combine(_tempOutputDirectory, tempCopyFileName);

            return tempCopyFilePath;
        }

        static BuildableExpressionExtensions()
        {
            Directory.CreateDirectory(_tempOutputDirectory);
        }
    }
}
