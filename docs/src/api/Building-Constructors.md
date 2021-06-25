[Classes](Building-Classes) and [structs](Building-Structs) can be given multiple constructors, and
constructors can be chained together.

To add a constructor, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add a struct:
    cs.AddStruct("MyStruct", str =>
    {
        // Add a constructor:
        str.AddConstructor(ctor =>
        {
            // Add an int parameter:
            ctor.AddParameter<int>("intValue");

            // Set an empty body for this example:
            ctor.SetBody(Expression.Empty());
        });
    });
});
```

## Chaining Constructors

To call one constructor from another, call:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add a class:
    cs.AddClass("MyClass", cls =>
    {
        // Add a constructor taking a double:
        var doubleCtor = cls.AddConstructor(ctor =>
        {
            // Add a double parameter:
            ctor.AddParameter<double>("doubleValue");

            // Set an empty body for this example:
            ctor.SetBody(Expression.Empty());
        });

        // Add a second constructor taking an int:
        cls.AddConstructor(ctor =>
        {
            // Add an int parameter:
            var intParameter = ctor.AddParameter<int>("intValue");

            // Call the doubleValue constructor from 
            // this constructor:
            ctor.SetConstructorCall(
                doubleCtor, 
                Expression.Convert(intParameter, typeof(double)));

            // Set an empty body for this example:
            ctor.SetBody(Expression.Empty());
        });
    });
});
```
