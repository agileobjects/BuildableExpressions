**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime, or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining an Interface

To add an interface to your `SourceCodeExpression`, use:

```cs
BuildableExpression.SourceCode(sc =>
{
    sc.AddInterface("IMyInterface", itf =>
    {
        // Set interface options if desired:
        // itf.AddAttribute<SomeAttribute>();
        // itf.SetVisibility(TypeVisibility.Internal);
        // itf.SetPartial();

        // Add interface members - see below
    });
});
```

Members are added via the interface API, which supports:

- [Properties](/api/Building-Properties)
- [Methods](/api/Building-Methods)
- [Attributes](/api/Building-Attributes)
- [Interface implementation](/api/Implementing-Interfaces)

## Implementing an Interface

For information on how to implement an interface, see [here](/api/Implementing-Interfaces).