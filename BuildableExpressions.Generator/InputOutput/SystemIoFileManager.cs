namespace AgileObjects.BuildableExpressions.Generator.InputOutput
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using static System.IO.SearchOption;

    internal class SystemIoFileManager : IFileManager
    {
        public static readonly IFileManager Instance = new SystemIoFileManager();

        public IEnumerable<string> FindFiles(string path, string pattern)
        {
            return Directory.Exists(path)
                ? Directory.EnumerateFiles(path, pattern, AllDirectories)
                : Array.Empty<string>();
        }

        public Stream OpenRead(string filePath) => File.OpenRead(filePath);

        public string Read(string filePath) => File.ReadAllText(filePath);

        public void Write(string filePath, string contents)
            => File.WriteAllText(filePath, contents);

        public void EnsureDirectory(string directory)
            => Directory.CreateDirectory(directory);
    }
}