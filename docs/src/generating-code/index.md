**BuildableExpressions.Generator** enables build-time C# source code file generation via a 
[configurable](/generating-code/Configuration) 
[MSBuild task](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-tasks). It works within
Visual Studio or from `dotnet build`, and supports 
[SDK](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview) and non-SDK projects.

To install from NuGet, use:

```shell
PM> Install-Package AgileObjects.BuildableExpressions.Generator -Pre
```
[![NuGet version](https://badge.fury.io/nu/AgileObjects.BuildableExpressions.Generator.svg)](https://badge.fury.io/nu/AgileObjects.BuildableExpressions.Generator)

## How it Works

To generate source code files, add the **AgileObjects.BuildableExpressions.Generator** NuGet package
to a project, then add a class which implements the `AgileObjects.BuildableExpressions.ISourceCodeExpressionBuilder`
interface:

```cs
interface ISourceCodeExpressionBuilder
{
    IEnumerable<SourceCodeExpression> Build(IExpressionBuildContext context);
}
```

When the project is built:

1. Types implementing `ISourceCodeExpressionBuilder` are discovered in the build output

2. An instance of each `ISourceCodeExpressionBuilder` type is created and its `.Build()` method is called

3. Any `SourceCodeExpression`s returned from `.Build()` method calls are converted to C# source-code strings

4. Any C# source-code strings are written to files in the target project

5. If any source code files are generated, the target project is built again, to include the generated types.<br />
   This second build skips steps 1-4.

## Rules

- Each `SourceCodeExpression` returned from an `ISourceCodeExpressionBuilder.Build()` method is given
  its own source code file, named after the first type added to the source code. If multiple types are
  added to the same `SourceCodeExpression`, the generated source code file will contain them all.

## Examples

These simple examples generate a set of `Greeter` C# source code files from a set of newline-separated
names in a `names.txt` file in the build output. Each `Greeter` implements the following `IGreeter` interface:

```cs
namespace Greetings
{
    public interface IGreeter
    {
        string Greet();
    }
}
```

### String -> SourceCode -> File

To create `Greeter` SourceCodeExpressions and files from C# source-code strings, use:

```cs
public class GreeterGenerator : ISourceCodeExpressionBuilder
{
    public IEnumerable<SourceCodeExpression> Build(
        IExpressionBuildContext context)
    {
        // Get the path to the names.txt file:
        var namesFilePath = Path.Combine(
            context.InputProjectOutputPath,
            "names.txt");

        // Read the file into a string collection:
        var namesInFile = File.ReadAllLines(namesFilePath);

        // For each name in the file, create a [Name]Greeter SourceCodeExpression:
        foreach (var nameInFile in namesInFile)
        {
            // Get a valid class name:
            var className = nameInFile.Replace(" ", "").Trim() + "Greeter";

            var sourceCodeCSharp = $@"
using Greetings;

public class {className} : IGreeter
{{
    public string Greet()
    {{
        return ""{nameInFile}"";
    }}
}}";
            yield return BuildableExpression.SourceCode(sourceCodeCSharp);
        }
    }
}
```

### Expression -> SourceCode -> File

To create `Greeter` SourceCodeExpressions and files from an Expression, use:

```cs
public class GreeterGenerator : ISourceCodeExpressionBuilder
{
    public IEnumerable<SourceCodeExpression> Build(
        IExpressionBuildContext context)
    {
        // Get the path to the names.txt file:
        var namesFilePath = Path.Combine(
            context.InputProjectOutputPath,
            "names.txt");

        // Read the file into a string collection:
        var namesInFile = File.ReadAllLines(namesFilePath);

        // For each name in the file, create a [Name]Greeter SourceCodeExpression:
        foreach (var nameInFile in namesInFile)
        {
            // Get a valid class name:
            var className = nameInFile.Replace(" ", "").Trim() + "Greeter";
            
            // Build a SourceCodeExpression using the API:
            yield return BuildableExpression.SourceCode(sc =>
            {
                // Define the [Name]Greeter class:
                sc.AddClass(className, cls =>
                {
                    // Implement the IGreeter interface:
                    cls.SetImplements<IGreeter>(impl =>
                    {
                        // Implement the .Greet() method:
                        impl.AddMethod(
                            nameof(IGreeter.Greet),
                            body: Expression.Constant(nameInFile));
                    });
                });
            });
        }
    }
}
```

More on the SourceCodeExpression API can be found [here](/api).

More on source code generation samples can be found [here](/generating-code/Samples).