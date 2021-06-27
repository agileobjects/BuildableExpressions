#if FEATURE_GENERATOR
namespace AgileObjects.BuildableExpressions.Generator.InputOutput
#else
namespace AgileObjects.BuildableExpressions.InputOutput
#endif
{
    using System;

    /// <summary>
    /// Attempts to delete all temporary files written to the BuildableExpressions output directory.
    /// </summary>
    internal class OutputCleaner : IDisposable
    {
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCleaner"/> class.
        /// </summary>
        /// <param name="fileManager">The <see cref="IFileManager"/> with which to delete temporary files.</param>
        public OutputCleaner(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        /// <summary>
        /// Attempts to delete all temporary files written to the BuildableExpressions output
        /// directory.
        /// </summary>
        public void Dispose()
        {
            var tempOutputFilePaths = _fileManager.FindFiles(
                _fileManager.TempOutputDirectory, "*.*");

            foreach (var tempOutputFilePath in tempOutputFilePaths)
            {
                try
                {
                    _fileManager.Delete(tempOutputFilePath);
                }
                catch
                {
                    // Try the next file
                }
            }
        }
    }
}