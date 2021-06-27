#if FEATURE_GENERATOR
namespace AgileObjects.BuildableExpressions.Generator.InputOutput
#else
namespace AgileObjects.BuildableExpressions.InputOutput
#endif
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implementing classes will provide directory and file services.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Gets the fully-qualified path to the location in the temp directory to which temp
        /// BuildableExpression output files are written.
        /// </summary>
        string TempOutputDirectory { get; }

        /// <summary>
        /// Finds directories below the directory at the given <paramref name="path"/> which match
        /// the given <paramref name="pattern"/>, optionally performing a <paramref name="recursive"/>
        /// search.
        /// </summary>
        /// <param name="path">The fully-qualified path to the directory to search.</param>
        /// <param name="pattern">The pattern with which to match sub-directores.</param>
        /// <param name="recursive">
        /// Whether to perform a recursive search through all sub-directories, or to only consider
        /// those directly below the directory at the given <paramref name="path"/>.
        /// </param>
        /// <returns>
        /// Zero or more fully-qualified paths to directories which match the given
        /// <paramref name="pattern"/>, below the given directory <paramref name="path"/>.
        /// </returns>
        IEnumerable<string> FindDirectories(
            string path,
            string pattern,
            bool recursive);

        /// <summary>
        /// Finds files below the directory at the given <paramref name="path"/> which match
        /// the given <paramref name="pattern"/>.
        /// </summary>
        /// <param name="path">The fully-qualified path to the directory to search.</param>
        /// <param name="pattern">The pattern with which to match files.</param>
        /// <returns>
        /// Zero or more fully-qualified paths to files which match the given
        /// <paramref name="pattern"/>, below the given directory <paramref name="path"/>.
        /// </returns>
        IEnumerable<string> FindFiles(string path, string pattern);

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
        string GetTempCopyFilePath(string fileName, string extension);

        /// <summary>
        /// Opens the file at the given fully-qualified <paramref name="filePath"/> for reading.
        /// </summary>
        /// <param name="filePath">The fully-qualified path to the file to open.</param>
        /// <returns>A readable stream to the file at the given <paramref name="filePath"/>.</returns>
        Stream OpenRead(string filePath);

        /// <summary>
        /// Gets a Version object detailing the version information of the file at the given
        /// fully-qualified <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The fully-qualified path to the file for which to retrieve a Version.</param>
        /// <returns>A
        /// Version object detailing the version information of the file at the given fully-qualified
        /// <paramref name="filePath"/>.
        /// </returns>
        Version GetVersion(string filePath);

        /// <summary>
        /// Opens the file at the given fully-qualified <paramref name="filePath"/> for writing.
        /// </summary>
        /// <param name="filePath">The fully-qualified path to the file to open.</param>
        /// <returns>A writable stream to the file at the given <paramref name="filePath"/>.</returns>
        Stream OpenWrite(string filePath);

        /// <summary>
        /// Writes the given <paramref name="content"/> to the file athe given fullt-qualified
        /// <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">
        /// The fully-qualified path to the file to which tow rite the <paramref name="content"/>.
        /// </param>
        /// <param name="content">The content to write to the file.</param>
        void Write(string filePath, string content);

        /// <summary>
        /// Creates a directory at the given fully-qualified <paramref name="directory"/> path, if
        /// one does not already exist.
        /// </summary>
        /// <param name="directory">The fully-qualified path at which to ensure a directory exists.</param>
        void EnsureDirectory(string directory);

        /// <summary>
        /// Delete the file at the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The fuly-qualified path to the file to delete.</param>
        void Delete(string filePath);
    }
}
