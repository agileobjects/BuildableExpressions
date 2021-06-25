**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining a Class

To add a class to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        // Set class attributes:
        cls.SetVisibility(TypeVisibility.Internal);
        cls.SetStatic();
        cls.SetAbstract();
        cls.SetSealed();
        cls.SetPartial();

        // Add class members
    });
});
```

The Class API supports:

- [Constructors](Building-Constructors)
- [Fields](Building-Fields)
- [Properties](Building-Properties)
- [Methods](Building-Methods)
- [Attributes](Building-Attributes)
- [Interface implementation](Implementing-Interfaces)
- [Inheritance](Implementing-Inheritance)