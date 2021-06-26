## Code Generation Target Project

By default, generated source code files are added to the project containing the `ISourceCodeExpressionBuilder`
implementation. To add files to a different project, add the following to the project containing the generators:

```xml
<PropertyGroup>
  <XprGeneratorOutputProject>[Path to target csproj file]</XprGeneratorOutputProject>
</PropertyGroup>
```

## Logging

By default, source code generation logs messages to the build output with the prefix 'XprGenerator'. 
To log messages with a different prefix, add the following to the project containing the generators:

```xml
<PropertyGroup>
  <XprGeneratorLoggerPrefix>[Your prefix]</XprGeneratorLoggerPrefix>
</PropertyGroup>
```

## Disable Code Generation

To disable code generation, add the following to the project containing the generators:

```xml
<PropertyGroup>
  <XprGenerator>False</XprGenerator>
</PropertyGroup>
```