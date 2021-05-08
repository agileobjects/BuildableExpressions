namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;

    internal class SystemIoFileManager : IFileManager
    {
        public static readonly IFileManager Instance = new SystemIoFileManager();

        public IEnumerable<string> FindFiles(string path, string pattern)
            => Directory.EnumerateFiles(path, pattern);

        public Stream OpenRead(string filePath) => File.OpenRead(filePath);

        public string Read(string filePath) => File.ReadAllText(filePath);

        public void Write(string filePath, string contents)
            => File.WriteAllText(filePath, contents);

        public void EnsureDirectory(string directory)
            => Directory.CreateDirectory(directory);
    }
}