To derive a generated [class](Building-Classes) from another, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add an abstract Animal class:
    var animal = sc.AddClass("Animal", cls =>
    {
        cls.SetAbstract();

        // Add an abstract int LegCount property:
        cls.AddProperty<int>("LegCount", p =>
        {
            p.SetAbstract();

            // Make get-only:
            p.SetGetter();
        });
    });

    // Add a Dog class derived from Animal:
    sc.AddClass("Dog", cls =>
    {
        // Set Animal as the Dog's base type:
        dog.SetBaseType(animal, impl =>
        {
            // Get a reference the abstract 'Animal.LegCount' property:
            var legCountProperty = 
                animal.PropertyExpressions.First();

            // Implement Animal.LegCount:
            impl.AddProperty(legCountProperty, p =>
            {
                // Implement the getter:
                p.SetGetter(gtr =>
                {
                    // Return 4!
                    gtr.SetBody(Expression.Constant(4));
                });
            });
        });
    });
});
```

### Base Constructor Calls

To call a base class constructor when generating a derived type, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add an abstract Animal class:
    var animal = sc.AddClass("Animal", cls =>
    {
        cls.SetAbstract();

        // Add a get-only int LegCount property:
        var legCountProperty = cls
            .AddProperty<int>("LegCount", p => p.SetGetter());

        cls.AddConstructor(ctor =>
        {
            // Abstract class constructor, so make it protected:
            ctor.SetVisibility(MemberVisibility.Protected);

            var legCountParam = ctor
                .AddParameter<int>("legCount");

            // Assign the LegCount property to the
            // legCount parameter - 'cls.ThisInstanceExpression'
            // provides access to the 'this' keyword:
            ctor.AddBody(Expression.Assign(
                Expression.Property(
                    cls.ThisInstanceExpression,
                    legCountProperty.PropertyInfo),
                legCountParam));
        });
    });

    // Add a Dog class derived from Animal:
    sc.AddClass("Dog", cls =>
    {
        // Set Animal as the Dog's base type:
        dog.SetBaseType(animal);

        // Add a constructor:
        dog.AddConstructor(ctor =>
        {
            // Get a reference to the base class constructor:
            var baseCtor = animal.ConstructorExpressions.First();

            // Get a value to pass for the 'legCount' parameter:
            var fourLegs = Expression.Constant(4);

            // Add the base constructor call:
            ctor.SetConstructorCall(baseCtor, fourLegs);
        });
    });
});
```