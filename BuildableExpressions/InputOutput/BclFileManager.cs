namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;

    internal class BclFileManager : IFileManager
    {
        public static readonly IFileManager Instance = new BclFileManager();

        public IEnumerable<string> FindFiles(string path, string pattern)
            => Directory.EnumerateFiles(path, pattern);

        public bool Exists(string filePath) => new FileInfo(filePath).Exists;

        public string Read(string filePath) => File.ReadAllText(filePath);

        public void Write(string filePath, string contents)
            => File.WriteAllText(filePath, contents);

        public void EnsureDirectory(string directory)
            => Directory.CreateDirectory(directory);
    }
}