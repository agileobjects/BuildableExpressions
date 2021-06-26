**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime, or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining a Class

To add a class to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        // Set class options if desired:
        // cls.AddAttribute(typeof(SomeAttribute));
        // cls.SetVisibility(TypeVisibility.Internal);
        // cls.SetStatic();
        // cls.SetAbstract();
        // cls.SetSealed();
        // cls.SetPartial();

        // Add class members
    });
});
```

The Class API supports:

- [Constructors](/api/Building-Constructors)
- [Fields](/api/Building-Fields)
- [Properties](/api/Building-Properties)
- [Methods](/api/Building-Methods)
- [Attributes](/api/Building-Attributes)
- [Interface implementation](/api/Implementing-Interfaces)
- [Inheritance](/api/Implementing-Inheritance)