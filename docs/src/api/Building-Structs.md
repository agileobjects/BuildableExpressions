**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime, or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining a Struct

To add a struct to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddStruct("MyStruct", str =>
    {
        // Set struct options if desired:
        // str.AddAttribute(typeof(SomeAttribute));
        // str.SetVisibility(TypeVisibility.Internal);
        // str.SetPartial();

        // Add struct members - see below
    });
});
```

Members are added via the struct API, which supports:

- [Constructors](/api/Building-Constructors)
- [Fields](/api/Building-Fields)
- [Properties](/api/Building-Properties)
- [Methods](/api/Building-Methods)
- [Attributes](/api/Building-Attributes)
- [Interface implementation](/api/Implementing-Interfaces)