To add a field to a [class](/api/Building-Classes) or [struct](/api/Building-Fields), use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        cls.AddField<string>("StringField", f =>
        {
            // Set field options:
            f.SetVisibility(MemberVisibility.Internal);
            f.SetStatic();
            f.SetReadonly();
            f.SetInitialValue("Hello world!");
        })
    });
});
```

The `AddField()` method returns a `FieldExpression` with a `FieldInfo` property which can be used 
to access the field from other Expressions.

For example, to assign a field in a [constructor](/api/Building-Constructors), use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddStruct("MyStruct", str =>
    {
        // Add a private int _count field:
        var countField = str.AddField<int>("_count", f =>
        {
            // Set field options:
            f.SetVisibility(MemberVisibility.Private);
            f.SetReadonly();
        });

        // Get a reference to the _count field -
        // str.ThisInstanceExpression provides 
        // access to the struct's 'this' keyword:
        var countFieldAccess = Expression.Field(
            str.ThisInstanceExpression,
            countField.FieldInfo);

        // Add a constructor:
        str.AddConstructor(ctor =>
        {
            // Add an int count parameter to the constructor:
            var countParam = ctor.AddParameter<int>("count");

            // Populate the _count field with the count parameter
            // value in the constructor body:
            ctor.SetBody(Expression.Assign(
                countFieldAccess,
                countParam));
        });

        // Add a get-only int Count property:
        str.AddProperty<int>("Count", p =>
        {
            // Return the _count field from the property getter:
            p.SetGetter(gtr =>
            {
                gtr.SetBody(countFieldAccess);
            });
        });
    });
});
```

Fields cannot be added to [interfaces](/api/Building-Interfaces).