﻿namespace AgileObjects.BuildableExpressions.ProjectManagement
{
    using System;
    using System.IO;
    using System.Linq;
    using Configuration;
    using InputOutput;
    using static System.StringComparison;

    internal class ProjectFactory : IProjectFactory
    {
        private readonly IFileManager _fileManager;

        public ProjectFactory(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public IProject GetProject(Config config)
        {
            using var fileReadStream = _fileManager.OpenRead(config.ProjectPath);
            using var fileReader = new StreamReader(fileReadStream);

            while (true)
            {
                var fileLine = fileReader.ReadLine();

                if (fileLine == null || char.IsWhiteSpace(fileLine.First()))
                {
                    throw new NotSupportedException(
                        $"Unable to find <Project /> element in file '{config.ProjectPath}'");
                }

                if (!fileLine.StartsWith("<Project", OrdinalIgnoreCase))
                {
                    continue;
                }

#if NETFRAMEWORK
                if (fileLine.IndexOf(" Sdk=\"", OrdinalIgnoreCase) != -1)
                {
                    return new SdkProject();
                }

                return new NetFrameworkProject(config);
#else
                return new SdkProject();
#endif
            }
        }
    }
}