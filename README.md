# BuildableExpressions

[![NuGet version](https://badge.fury.io/nu/AgileObjects.BuildableExpressions.svg)](https://badge.fury.io/nu/AgileObjects.BuildableExpressions)

BuildableExpressions is a library which generates .NET Types at runtime.

BuildableExpressions.Generator is a Nuget-packaged [MSBuild task](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-tasks)
which generates C# source code files at build-time. It works from `dotnet build` or from within Visual Studio, and works with 
[SDK](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview) and non-SDK projects.

Both packages generate from C# source-code strings or
[Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees), and target .NET 4.6.1
and [.NETStandard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard). Both are available via NuGet and licensed with the 
[MIT licence](LICENCE.md). Check out [the documentation](https://buildableexpressions.readthedocs.io) for more!