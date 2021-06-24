**BuildableExpressions** and **BuildableExpressions.Generator** work via creation of 
`SourceCodeExpression`s, which can be compiled to CLR Types at runtime or used to generate C# source
code files at build-time. A `SourceCodeExpression` consists of one or more types.

## Defining an Enum

To add an enum to your `SourceCodeExpression`, use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Add an enum named 'Numbers' with the members
    // Zero, One, Two and Three. The member numeric 
    // values will be set to 0, 1, 2 and 3 automatically:
    sc.AddEnum("Numbers", "Zero", "One", "Two", "Three");

    // Add an enum named 'OddNumbers':
    sc.AddEnum("OddNumbers", enm =>
    {
        // Add the enum members and their numeric values:
        enm.AddMember("Two", 2);
        enm.AddMember("Four", 4);
        enm.AddMember("Six", 6);
    });
});
```