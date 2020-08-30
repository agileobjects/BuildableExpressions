namespace AgileObjects.BuildableExpressions.InputOutput
{
    using System.Collections.Generic;

    internal interface IFileManager
    {
        IEnumerable<string> FindFiles(string path, string pattern);

        bool Exists(string filePath);

        string Read(string filePath);

        void Write(string filePath, string contents);

        void EnsureDirectory(string directory);
    }
}
