**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining an Attribute

To add an attribute to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    sc.AddAttribute("MyAttribute", attr =>
    {
        // Set attribute attributes:
        attr.SetAbstract();
        attr.SetSealed();
        attr.SetPartial();

        // Add attribute members
    });
});
```

The Attribute API supports:

- [Constructors](Building-Constructors)
- [Fields](Building-Fields)
- [Properties](Building-Properties)
- [Methods](Building-Methods)
- Attributes
- [Interface implementation](Implementing-Interfaces)
- [Inheritance](Implementing-Inheritance)

## Applying Attributes

To apply an Attribute, use the following - normal CLR attributes can be applied as well as Attributes
you define in a SourceCodeExpression:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Make an empty marker attribute:
    var myAttribute = sc.AddAttribute("MarkerAttribute");

    sc.AddClass("AttributedClass", cls =>
    {
        // Mark AttributedClass with the MarkerAttribute:
        cls.AddAttribute(myAttribute);

        // Add an int field:
        cls.AddField<int>("AttributedField", f =>
        {
            // Mark AttributedField with the MarkerAttribute:
            f.AddAttribute(myAttribute);
        });

        // Add a string get/set auto property:
        cls.AddProperty<string>("AttributedAutoProperty", p =>
        {
            // Mark AttributedAutoProperty with the 
            // System.ComponentModel.DataAnnotations.RequiredAttribute:
            p.AddAttribute<RequiredAttribute>();
        });

        // Add an string get-only property:
        cls.AddProperty<int>("AttributedGetOnlyProperty", p =>
        {
            // Set the property's getter:
            p.SetGetter(gtr =>
            {
                // Mark AttributedGetOnlyProperty getter
                // with the MarkerAttribute:
                gtr.AddAttribute<RequiredAttribute>();

                // Set the AttributedGetOnlyProperty getter's body:
                gtr.SetBody(Expression.Constant(123));
            });
        });

        // Add a method:
        cls.AddMethod("AttributedMethod", m =>
        {
            // Mark AttributedMethod with the MarkerAttribute:
            m.AddAttribute(myAttribute);

            // Add an int parameter:
            m.AddParameter<int>("intValue", p =>
            {
                // Mark the intValue parameter with the MarkerAttribute:
                p.AddAttribute(myAttribute);
            });

            // Set the method body:
            m.SetBody(Expression.Empty());
        });
    });
});
```

## Setting Attribute Usage

To customise to which targets an attribute can be applied, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add an attribute which can only be applied to
    // classes, structs or interfaces:
    sc.AddAttribute("TypesOnlyAttribute", attr =>
    {
        // Set multiple AttributeTargets values:
        attr.SetValidOn(
            AttributeTargets.Class | 
            AttributeTargets.Struct | 
            AttributeTargets.Interface);
    });
});
```