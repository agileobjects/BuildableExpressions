#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement.NonSdk
{
    using System.Collections;
    using System.Collections.Generic;
    using EnvDTE;

    internal static class EnvDteExtensions
    {
        public static IEnumerable<Project> EnumerateProjects(this Solution solution)
            => Enumerate(solution.Projects);

        private static IEnumerable<Project> Enumerate(IEnumerable items)
        {
            foreach (var item in items)
            {
                var candidateProject = item;

                if (candidateProject is ProjectItem { SubProject: { } } projectItem)
                {
                    candidateProject = projectItem.SubProject;
                }

                if (candidateProject is not Project project)
                {
                    continue;
                }

                yield return project;

                try
                {
                    if (project.ProjectItems == null)
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                foreach (var subProject in Enumerate(project.ProjectItems))
                {
                    yield return subProject;
                }
            }
        }
    }
}
#endif