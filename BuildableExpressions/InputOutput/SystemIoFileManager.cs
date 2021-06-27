#if FEATURE_GENERATOR
namespace AgileObjects.BuildableExpressions.Generator.InputOutput
#else
namespace AgileObjects.BuildableExpressions.InputOutput
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using static System.IO.SearchOption;

    /// <summary>
    /// An <see cref="IFileManager"/> implementation using classes from the System.IO namespace.
    /// </summary>
    internal class SystemIoFileManager : IFileManager
    {
        /// <summary>
        /// Gets the singleton <see cref="SystemIoFileManager"/> instance.
        /// </summary>
        public static readonly IFileManager Instance;

        private static readonly string _tempOutputDirectory;

        static SystemIoFileManager()
        {
            Instance = new SystemIoFileManager();

            _tempOutputDirectory = Path.Combine(
                Path.GetDirectoryName(Path.GetTempFileName())!,
                nameof(BuildableExpressions));

            Instance.EnsureDirectory(_tempOutputDirectory);
        }

        /// <inheritdoc />
        public string TempOutputDirectory => _tempOutputDirectory;

        /// <inheritdoc />
        public IEnumerable<string> FindDirectories(
            string path,
            string pattern,
            bool recursive)
        {
            return Directory.Exists(path)
                ? Directory.EnumerateDirectories(
                    path,
                    pattern,
                    recursive ? AllDirectories : TopDirectoryOnly)
                : Array.Empty<string>();
        }

        /// <inheritdoc />
        public IEnumerable<string> FindFiles(string path, string pattern)
        {
            return Directory.Exists(path)
                ? Directory.EnumerateFiles(path, pattern, AllDirectories)
                : Array.Empty<string>();
        }

        /// <inheritdoc />
        public string GetTempCopyFilePath(string fileName, string extension)
        {
            var tempFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            var tempCopyFileName = fileName + "_" + tempFileName + extension;
            var tempCopyFilePath = Path.Combine(_tempOutputDirectory, tempCopyFileName);

            return tempCopyFilePath;
        }

        /// <inheritdoc />
        public Stream OpenRead(string filePath) => File.OpenRead(filePath);

        /// <inheritdoc />
        public Version GetVersion(string filePath)
            => Version.Parse(FileVersionInfo.GetVersionInfo(filePath).FileVersion);

        /// <inheritdoc />
        public Stream OpenWrite(string filePath) => File.OpenWrite(filePath);

        /// <inheritdoc />
        public void Write(string filePath, string contents)
            => File.WriteAllText(filePath, contents);

        /// <inheritdoc />
        public void EnsureDirectory(string directory)
            => Directory.CreateDirectory(directory);

        /// <inheritdoc />
        public void Delete(string filePath) => File.Delete(filePath);
    }
}