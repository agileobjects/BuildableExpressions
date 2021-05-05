namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;
    using System.IO;

    internal interface IFileManager
    {
        IEnumerable<string> FindFiles(string path, string pattern);

        Stream OpenRead(string filePath);

        string Read(string filePath);

        void Write(string filePath, string contents);

        void EnsureDirectory(string directory);
    }
}
