namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using System;
    using System.IO;
    using Configuration;
    using Extensions;
    using InputOutput;
#if NETFRAMEWORK
    using NonSdk;
#endif
    using static System.StringComparison;

    internal class ProjectFactory : IProjectFactory
    {
        private readonly IFileManager _fileManager;

        public ProjectFactory(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public IProject GetOutputProjectOrThrow(IConfig config)
        {
            var outputProject = config.OutputProjectPath;
            var rootNamespace = Path.GetFileNameWithoutExtension(outputProject);

            var projectFactory = default(Func<IConfig, string, IProject>);

            using var fileReadStream = _fileManager.OpenRead(outputProject);
            using var fileReader = new StreamReader(fileReadStream);

            while (true)
            {
                var fileLine = fileReader.ReadLine();

                if (fileLine == null)
                {
                    if (projectFactory != null)
                    {
                        break;
                    }

                    throw new NotSupportedException(
                        $"Unable to find <Project /> element in file '{outputProject}'");
                }

                if (fileLine.StartsWithIgnoreCase("<Project"))
                {
#if NETFRAMEWORK
                    if (fileLine.IndexOf(" Sdk=\"", OrdinalIgnoreCase) == -1)
                    {
                        projectFactory = (cfg, ns) => new NetFrameworkProject(cfg, ns);
                        continue;
                    }
#endif
                    projectFactory = (cfg, ns) => new SdkProject(cfg, ns);
                    continue;
                }

                if (fileLine.TrimStart().StartsWithIgnoreCase("<RootNamespace"))
                {
                    rootNamespace = fileLine
                        .Substring(fileLine.IndexOf('>') + 1)
                        .Split('<')[0];

                    break;
                }
            }

            return projectFactory!.Invoke(config, rootNamespace);
        }
    }
}