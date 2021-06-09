#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.UnitTests.ProjectManagement.NonSdk
{
    using System.Linq;
    using BuildableExpressions.UnitTests.Common;
    using Generator.ProjectManagement.NonSdk;
    using Xunit;

    public class DevToolsFactoryTests
    {
        [Fact]
        public void ShouldRetrieveSolutionProjects()
        {
            var devTools = DevToolsFactory
                .GetDevToolsOrNullFor("BuildableExpressions.sln");

            var projectNames = devTools
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