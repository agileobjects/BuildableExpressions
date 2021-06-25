To implement an interface in a [class](Building-Classes), [struct](Building-Structs) or other 
[interface](Building-Interfaces), use:

```csharp
BuildableExpression.SourceCode(sc =>
{
    // Define a custom interface type:
    var messagerInterface = sc.AddInterface("IMessager", itf =>
    {
        // Make IMessager implement IDisposable:
        itf.SetImplements<IDisposable>();

        // Add a single, void method:
        itf.AddMethod("SendMessage", typeof(void), m =>
        {
            // Add a single string method parameter:
            m.AddParameter<string>("message");
        });
    });

    // Add a class to implement some interfaces:
    sc.AddClass("Implementer", cls =>
    {
        // Implement the custom interface:
        cls.SetImplements(messagerInterface, impl =>
        {
            // Implement the IMessager.SendMessage() method:
            impl.AddMethod("SendMessage", m =>
            {
                // Empty body for this example:
                m.SetBody(Expression.Empty());
            });

            // Implement the IDisposable.Dispose() method:
            impl.AddMethod("Dispose", m =>
            {
                // Empty body for this example:
                m.SetBody(Expression.Empty());
            });
        });
    });
});
```