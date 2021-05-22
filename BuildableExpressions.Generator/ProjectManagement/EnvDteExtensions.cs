#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement
{
    using EnvDTE;
    using System.Collections;
    using System.Collections.Generic;

    internal static class EnvDteExtensions
    {
        public static IEnumerable<Project> EnumerateProjects(this Solution solution)
            => Enumerate(solution.Projects);

        private static IEnumerable<Project> Enumerate(IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is Project project)
                {
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

                    continue;
                }

                if (item is ProjectItem projectItem && projectItem.SubProject != null)
                {
                    yield return projectItem.SubProject;
                }
            }
        }
    }
}
#endif