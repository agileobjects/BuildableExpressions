The **BuildableExpressions** configuration API enables creation of [classes](/api/Building-Classes), 
[structs](/api/Building-Structs), [interfaces](/api/Building-Interfaces), [enums](/api/Building-Enums) 
and [attributes](/api/Building-Attributes), with Expressions used to implement method bodies and 
property accessors.

## Expressions vs Strings

The `BuildableExpression.SourceCode()` factory method can also take a C# source-code string, which
depending on your scenario could be simpler and faster.

So why use Expressions?

- To implement methods using complex
  [Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees) 
  built by existing code - perhaps to move runtime logic to build-time

- To write more refactor-friendly code - source code strings cannot be statically checked and have
  no design-time safety

- Expression Tree code compilation or generation handles referencing required assemblies for you