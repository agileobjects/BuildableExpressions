**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining a Struct

To add a struct to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddStruct("MyStruct", str =>
    {
        // Set struct attributes:
        str.SetPartial();

        // Add struct members
    });
});
```

The Struct API supports:

- [Constructors](Building-Constructors)
- [Fields](Building-Fields)
- [Properties](Building-Properties)
- [Methods](Building-Methods)
- [Attributes](Building-Attributes)
- [Interface implementation](Implementing-Interfaces)