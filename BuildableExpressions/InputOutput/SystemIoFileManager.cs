namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SourceCode.Extensions;
    using static System.IO.SearchOption;

    internal class SystemIoFileManager : IFileManager
    {
        public static readonly IFileManager Instance = new SystemIoFileManager();

        public IEnumerable<string> FindFiles(string path, string pattern)
        {
            var wildcardIndex = path.IndexOf('*');

            if (wildcardIndex == -1)
            {
                return Directory.Exists(path)
                    ? Directory.EnumerateFiles(path, pattern, AllDirectories)
                    : Enumerable<string>.EmptyArray;
            }

            var rootDirectory = Path.GetDirectoryName(path.Substring(0, wildcardIndex));

            if (!Directory.Exists(rootDirectory))
            {
                return Enumerable<string>.EmptyArray;
            }

            var searchDirectories = path.Substring(rootDirectory!.Length);
            var wildcardDirectory = Path.GetDirectoryName(searchDirectories);
            var searchSubDirectories = searchDirectories.Substring(wildcardDirectory!.Length);

            wildcardDirectory = RemoveLeadingDirectorySeparator(wildcardDirectory);
            searchSubDirectories = RemoveLeadingDirectorySeparator(searchSubDirectories);

            return Directory
                .EnumerateDirectories(rootDirectory, wildcardDirectory)
                .SelectMany(directory => FindFiles(
                    Path.Combine(directory, searchSubDirectories),
                    pattern));
        }

        private static string RemoveLeadingDirectorySeparator(string directory)
            => directory.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        public Stream OpenRead(string filePath) => File.OpenRead(filePath);

        public string Read(string filePath) => File.ReadAllText(filePath);

        public void Write(string filePath, string contents)
            => File.WriteAllText(filePath, contents);

        public void EnsureDirectory(string directory)
            => Directory.CreateDirectory(directory);
    }
}