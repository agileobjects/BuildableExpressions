Generic parameters can be added to [classes](Building-Classes), [structs](Building-Structs), 
[interfaces](Building-Interfaces) and [methods](Building-Methods).

To add a generic parameter to a type, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add a class:
    sc.AddClass("ValueWrapper", cls =>
    {
        // Add a generic type parameter:
        var genericParam = cls
            .AddGenericParameter("TValue");

        // Add a public get-set Value property with 
        // the generic type parameter as its type:
        cls.AddProperty("Value", genericParam);
    });
});
```

### Type Constraints

To add type constraints to a generic parameter, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add a struct:
    sc.AddStruct("ValueWrapper", str =>
    {
        // Add a constrained generic type parameter:
        str.AddGenericParameter("TConstrained", gp =>
        {
            // Set constraint options:
            gp.AddClassConstraint();
            gp.AddStructConstraint();
            gp.AddNewableConstraint();

            // Add type contraints:
            gp.AddTypeConstraint<IDisposable>();
            gp.AddTypeConstraints(typeof(MyClass), typeof(MyOtherClass));
        });
    });
});
```

## Closing Generic Parameters

To close an open generic parameter to a generic argument, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add a class to implement IDictionary<TKey, TValue>:
    sc.AddClass("StringKeyedDictionary", cls =>
    {
        // Add a TValue generic parameter - this will be 
        // used for IDictionary<,>'s TValue parameter:
        cls.AddGenericParameter("TValue");

        // Implement IDictionary<,>:
        cls.SetImplements(typeof(IDictionary<,>), impl =>
        {
            // Close the TKey generic parameter to string:
            cls.SetGenericArgument<string>("TKey");
        });
    });
});
```