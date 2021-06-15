﻿namespace AgileObjects.BuildableExpressions.Generator.InputOutput
{
    using System.Collections.Generic;
    using System.IO;

    internal interface IFileManager
    {
        IEnumerable<string> FindDirectories(
            string path,
            string pattern,
            bool recursive);

        IEnumerable<string> FindFiles(string path, string pattern);

        Stream OpenRead(string filePath);

        string GetVersion(string filePath);

        Stream OpenWrite(string filePath);

        void Write(string filePath, string contents);

        void EnsureDirectory(string directory);
    }
}
