To add a triple-slash XML documentation summary to a generated [class](/api/Building-Classes), 
[struct](/api/Building-Struct), [interface](/api/Building-Interfaces), [enum](/api/Building-Enums), 
[attribute](/api/Building-Attributes), [field](/api/Building-Fields), [property](/api/Building-Properties) 
or [method](/api/Building-Methods), use:

```cs
BuildableExpression.SourceCode(sc =>
{
    sc.AddClass("MyClass", cls =>
    {
        cls.SetSummary(
            "This is my generated class, " + 
            "and it's pretty special");
    });
});
```

## File Headers

To add a file header to a generated c# source code file, use:

```cs
BuildableExpression.SourceCode(sc =>
{
    sc.SetHeader($@"
// - - - - - - - - - - - - - - - - - - - - -
// This is generated code.
// Generated on: {DateTime.Now}
// Don't change this file directly, as your
// changes may be overwritten.
// - - - - - - - - - - - - - - - - - - - - -");
});
```
