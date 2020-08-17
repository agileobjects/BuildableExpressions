namespace AgileObjects.BuildableExpressions.InputOutput
{
    internal interface IFileManager
    {
        bool Exists(string filePath);

        string Read(string filePath);

        void Write(string filePath, string contents);
    }
}
