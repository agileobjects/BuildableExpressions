**BuildableExpressions** enables CLR type generation at runtime from C# source-code strings or
[Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees).

To install from NuGet, use:

    PM> Install-Package AgileObjects.BuildableExpressions

### Examples

These simple examples create a `Greeter` type which implements the following `IGreeter` interface:

```csharp
namespace Greetings
{
    public interface IGreeter
    {
        string Greet();
    }
}
```

### String -> SourceCode -> Type

To create the `HelloWorldGreeter` SourceCodeExpression and Type from a C# source-code string, use:

```csharp
// Define a C# source-code string:
const string sourceCodeCSharp = @"
using Greetings;

public class HelloWorldGreeter : IGreeter
{
    public string Greet()
    {
        return ""Hello world!"";
    }
}";

// Create a SourceCodeExpression from the source-code string:
var sourceCode = BuildableExpression.SourceCode(sourceCodeCSharp);

// Compile the SourceCodeExpression to its CLR Types -
// pass the IGreeter type's Assembly to enable compilation:
var greeterType = sourceCode
    .CompileToTypes(referenceAssemblies: typeof(IGreeter).Assembly)
    .First();

// Create an instance of the HelloWorldGreeter as an IGreeter:
var helloWorldGreeter = 
    (IGreeter)Activator.CreateInstance(greeterType);

// Call .Greet() - prints 'Hello world!':
Console.WriteLine(helloWorldGreeter.Greet());
```

### Expression Tree -> SourceCode -> Type

To create the `HelloWorldGreeter` SourceCodeExpression and Type from an Expression Tree, use:

```csharp
// Build a SourceCodeExpression using the API:
var sourceCode = BuildableExpression
    .SourceCode(sc =>
    {
        // Define the HelloWorldGreeter class:
        sc.AddClass("HelloWorldGreeter", cls =>
        {
            // Implement the IGreeter interface:
            cls.SetImplements<IGreeter>(impl =>
            {
                // Implement the .Greet() method:
                impl.AddMethod(
                    nameof(IGreeter.Greet),
                    body: Expression.Constant("Hello world!"));
            });
        });
    });

// Compile the SourceCodeExpression to its CLR Types -
// unlike the String -> SourceCodeExpression example, 
// reference Assemblies are not required:
var greeterType = sourceCode.CompileToTypes().First();

// Create an instance of the HelloWorldGreeter as an IGreeter:
var helloWorldGreeter = 
    (IGreeter)Activator.CreateInstance(greeterType);

// Call .Greet() - prints 'Hello world!':
Console.WriteLine(helloWorldGreeter.Greet());
```

More on the SourceCodeExpression API can be found [here](/api).