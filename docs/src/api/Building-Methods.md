To add a method to a [class](/api/Building-Classes), [struct](/api/Building-Structs) or 
[interface](/api/Building-Interface), use:

```cs
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        // Add a method to triple a given value:
        cls.AddMethod("Triple", m =>
        {
            // Set method options if desired:
            // m.SetSummary("Method description");
            // m.AddAttribute(typeof(SomeAttribute));
            // m.SetVisibility(MemberVisibility.Internal);
            // m.SetStatic();
            // m.SetAbstract();
            // m.SetVirtual();

            // Get an int parameter:
            var intParam = 
                Expression.Parameter(typeof(int), "intValue");

            // Set the method's definition to a LambdaExpression
            // multiplying the passed-in int value by 3. The method's
            // parameters will match those of the given Lambda:
            m.SetDefinition(Expression.Lambda(
                Expression.Multiply(
                    intParam,
                    Expression.Constant(3)),
                intParam));
        });
    });
});
```

### Overriding

To override an abstract or virtual method, use:

```cs
BuildableExpression.SourceCode(sc =>
{
    // Store a reference to the method to override
    // in a local variable:
    var baseIsMagicMethod = default(MethodExpression);

    // Add a base class:
    var baseClass = sc.AddClass("BaseClass", cls =>
    {
        // Add a parameterless, virtual method to override -
        // update the local reference to the added MethodExpression:
        baseIsMagicMethod = cls.AddMethod<bool>("IsMagic", m =>
        {
            // Mark as virtual (or abstract):
            m.SetVirtual(); // m.SetAbstract();

            // Return false from the default method body:
            m.SetBody(Expression.Constant(false));
        });
    });

    sc.AddClass("DerivedClass", cls =>
    {
        // Derive this class from the base class:
        cls.SetBaseType(baseClass, impl =>
        {
            // Override the base class method by passing
            // it to the AddMethod() call:
            impl.AddMethod(baseIsMagicMethod, m =>
            {
                // Return true from the overridden method body:
                m.SetBody(Expression.Constant(true));
            })
        });
    });
});
```