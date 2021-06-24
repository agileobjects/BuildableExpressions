--8<-- "_api-intro.md"

To add a class to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
	sc.AddClass("MyClass", cls =>
	{
		// Set class attributes:
		cls.SetStatic();
		cls.SetAbstract();
		cls.SetSealed();

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
- [Implementing interfaces](Implementing-Interfaces)
- [Inheritance](Implementing-Inheritance)