The **BuildableExpressions** configuration API enables creation of [classes](Building-Classes), 
[structs](Building-Structs), [interfaces](Building-Interfaces), [enums](Building-Enums) and
[attributes](Building-Attributes), with Expressions used to implement method bodies and property 
accessors.

## Expressions vs Strings

The `BuildableExpression.SourceCode()` factory method can also take a C# source-code string, so why
use Expressions?

- To use complex [Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees) 
  built by existing code as method implementations

- To write more refactor-friendly code - source code strings cannot be statically checked and have
  no design-time safety