#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.UnitTests.ProjectManagement
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using BuildableExpressions.ProjectManagement;
    using Common;
    using EnvDTE;
    using Xunit;

    public class EnvDteExtensionsTests
    {
        [Fact]
        public void ShouldRetrieveSolutionProjects()
        {
            var dte = (DTE)Marshal.GetActiveObject("VisualStudio.DTE");

            var projects = dte
                .Solution
                .EnumerateProjects()
                .ToList();

            var expectedProjects = new[]
            {
                "BuildableExpressions",
                "BuildableExpressions.UnitTests",
                "BuildableExpressions.UnitTests.Common",
                "BuildableExpressions.UnitTests.NetCore3",
                "BuildableExpressions.Console",
                "BuildableExpressions.Console.NetCore3",
            };

            foreach (var projectName in expectedProjects)
            {
                projects
                    .Any(p => p.Name == projectName)
                    .ShouldBeTrue($"Project '{projectName}' not found");
            }
        }
    }
}
#endif