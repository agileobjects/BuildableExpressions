**BuildableExpressions** is a library which [generates .NET Types](/building-types/) at runtime.

[![NuGet version](https://badge.fury.io/nu/AgileObjects.BuildableExpressions.svg)](https://badge.fury.io/nu/AgileObjects.BuildableExpressions)

To install from NuGet, use:

```shell
PM> Install-Package AgileObjects.BuildableExpressions
```

**BuildableExpressions.Generator** is an [MSBuild task](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-tasks)
which [generates C# source code](/generating-code/) files at build-time. It works from `dotnet build` or from within Visual Studio, and 
supports [SDK](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview) and non-SDK projects.

[![NuGet version](https://badge.fury.io/nu/AgileObjects.BuildableExpressions.Generator.svg)](https://badge.fury.io/nu/AgileObjects.BuildableExpressions.Generator)

To install from NuGet, use:

```shell
PM> Install-Package AgileObjects.BuildableExpressions.Generator
```

Both packages generate from C# source-code strings or
[Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees), target .NET 4.6.1
and [.NETStandard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard), and are available under the 
[MIT licence](https://github.com/agileobjects/BuildableExpressions/blob/master/LICENCE.md). 

