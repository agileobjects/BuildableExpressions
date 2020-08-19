namespace AgileObjects.BuildableExpressions.Configuration
{
    internal interface IConfigManager
    {
        string ConfigFileName { get; }

        Config GetConfigOrNull(string contentRoot);
    }
}
