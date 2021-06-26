To add a property to a [class](/api/Building-Classes), [struct](/api/Building-Structs) or 
[interface](/api/Building-Interface), use:

```cs
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        // Add an int backing field:
        var intField = cls.AddField<int>("_intValue");

        // Get a reference to the backing field -
        // 'cls.ThisInstanceExpression' provides 
        // access to the 'this' keyword':
        var intFieldAccess = Expression.Field(
            cls.ThisInstanceExpression, 
            intField.FieldInfo);

        // Add an int property:
        cls.AddProperty<int>("IntValue", p =>
        {
            // Set property options if desired:
            // p.AddAttribute(typeof(SomeAttribute));
            // p.SetVisibility(MemberVisibility.Internal);
            // p.SetStatic();
            // p.SetAbstract();
            // p.SetVirtual();

            // Add a property getter [optional]:
            p.SetGetter(gtr =>
            {
                // Set getter options if desired:
                // gtr.AddAttribute<SomeAttribute>();
                // gtr.SetVisibility(MemberVisibility.Internal);

                // Return the value of the backing field
                // from the property getter:
                gtr.SetBody(intFieldAccess);
            });
            
            // Add a property setter [optional]:
            p.SetSetter(str =>
            {
                // Set setter options if desired:
                // str.AddAttribute<SomeAttribute>();
                // str.SetVisibility(MemberVisibility.Protected);

                // If the passed value is greater than zero, 
                // update the value of the backing field -
                // the lambda 'value' parameter is a ParameterExpression
                // providing access to the property setter's
                // 'value' keyword:
                str.SetBody(value => Expression.IfThen(
                    Expression.GreaterThan(
                        value, 
                        Expression.Constant(0),
                    Expression.Assign(
                        intFieldAccess,
                        value))));
            });
        });        
    });
});
```

### Auto-Properties

To add auto properties, use:

```cs
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        // Add a public string get-set auto property:
        cls.AddProperty<string>("Name");

        // Add an auto property with different accessor 
        // visibilities:
        cls.AddProperty<int>("IntProperty", p =>
        {
            // Set property options:
            p.SetAutoProperty(
                setterVisibility: MemberVisibility.Private);
        });        
    });
});
```

### Overriding

To override an abstract or virtual property, use:

```cs
BuildableExpression.SourceCode(sc =>
{
    // Add an abstract base class:
    var baseClass = sc.AddClass("BaseClass", cls =>
    {
        cls.SetAbstract();

        // Add an abstract property to override:
        cls.AddProperty<bool>("IsMagic", p =>
        {
            // Mark as abstract and get-only:
            p.SetAbstract();
            p.SetGetter();
        });
    });

    sc.AddClass("DerivedClass", cls =>
    {
        // Derive this class from the base class:
        cls.SetBaseType(baseClass, impl =>
        {
            // Get a reference to the base class property:
            var baseIsMagicProperty = 
                baseClass.PropertyExpressions.First();

            // Override the base class property by passing
            // it to the AddProperty() call:
            impl.AddProperty(baseIsMagicProperty, p =>
            {
                // Add a getter to implement the abstract 
                // base class getter:
                p.SetGetter(gtr =>
                {
                    gtr.SetBody(Expression.Constant(true));
                });
            })
        });
    });
});
```