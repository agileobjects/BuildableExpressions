**BuildableExpressions.Generator** sample projects can be found 
[here](https://github.com/agileobjects/BuildableExpressions/tree/master/BuildableExpressions.Generator.Samples),
demonstrating:

- Code generation via a variety of C# project and framework types
- Customised [output projects](/generating-code/Configuration/#code-generation-target-project) and
  [logging](/generating-code/Configuration/#logging)
- Building `SourceCodeExpression`s from C# source code strings and the BuildableExpressions [API](/api/).

The below gif shows code generation in the 
[CsvGenerator](https://github.com/agileobjects/BuildableExpressions/tree/master/BuildableExpressions.Generator.Samples/CsvGenerator) 
sample, a project which generates classes from CSV files and outputs them to a separate Models project.

The gif shows:

- The input CSV files
- Output files being generating by the build
- The target project being rebuilt with its new, generated classes
- The generated output files

![CsvGenerator](https://github.com/agileobjects/BuildableExpressions/blob/master/BuildableExpressions.Generator.Samples/CsvGenerator/CsvGenerator.gif)