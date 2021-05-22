#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.UnitTests.ProjectManagement
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using BuildableExpressions.Generator.ProjectManagement;
    using BuildableExpressions.UnitTests.Common;
    using EnvDTE;
    using Xunit;

    public class EnvDteExtensionsTests
    {
        [Fact]
        public void ShouldRetrieveSolutionProjects()
        {
            var dte = (DTE)Marshal.GetActiveObject("VisualStudio.DTE");

            var projectNames = dte
                .Solution
                .EnumerateProjects()
                .Select(p => p.Name)
                .ToList();

            var expectedProjects = new[]
            {
                "BuildableExpressions",
                "BuildableExpressions.Generator",
                "BuildableExpressions.Generator.UnitTests",
                "BuildableExpressions.Generator.UnitTests.NetCore2",
                "BuildableExpressions.UnitTests",
                "BuildableExpressions.UnitTests.Common",
                "BuildableExpressions.UnitTests.NetCore2",
                "BuildableExpressions.UnitTests.NetCore3",
                "BuildableExpressions.Console.Net461",
                "BuildableExpressions.Console.Net461.Sdk",
                "BuildableExpressions.Console.NetCore3",
            };

            foreach (var projectName in expectedProjects)
            {
                projectNames
                    .Any(name => name == projectName)
                    .ShouldBeTrue($"Project '{projectName}' not found");
            }
        }
    }
}
#endif