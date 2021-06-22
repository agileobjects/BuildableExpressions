// This generator outputs Category.cs and Product.cs to the CsvGenerator.Models
// project based on the contents of the Categories.csv and Products.csv files.

// The generator output target is set by this project's XprGeneratorOutputProject property.

// This sample is based on the .NET 5 Source Generators sample:
// https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/CsvGenerator.cs

namespace AgileObjects.BuildableExpressions.Generator.Samples.CsvGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions;
    using NotVisualBasic.FileIO;
    using SourceCode;
    using static System.StringComparison;

    public class CsvToModelsGenerator : ISourceCodeExpressionBuilder
    {
        public IEnumerable<SourceCodeExpression> Build(IExpressionBuildContext context)
        {
            var csvParsers = Directory
                .EnumerateFiles(context.InputProjectOutputPath, "*.csv")
                .Select(csvFilePath => (
                    Path.GetFileName(csvFilePath),
                    GetClassName(csvFilePath),
                    new CsvTextFieldParser(File.OpenRead(csvFilePath))));

            foreach (var (fileName, className, parser) in csvParsers)
            {
                var (types, names, values) = GetClassProperties(parser);
                var minLen = Math.Min(types.Length, names.Length);

                yield return BuildableExpression.SourceCode(sc =>
                {
                    sc.SetHeader("Generated from CSV file " + fileName);

                    sc.AddClass(className, cls =>
                    {
                        for (var i = 0; i < minLen; i++)
                        {
                            cls.AddProperty(names[i], types[i]);
                        }

                        var classConstructor = cls.Type.GetConstructor(Type.EmptyTypes);
                        var classProperties = cls.Type.GetProperties();

                        var classListType = typeof(List<>).MakeGenericType(cls.Type);

                        cls.AddField("All", classListType, fld =>
                        {
                            fld.SetStatic();
                            fld.SetReadonly();

                            var classObjects = new List<Expression>();

                            do
                            {
                                if (values == null)
                                {
                                    // The CSV parser pre-reads one line of data from the CSV
                                    // so it can figure out what types the properties are - 
                                    // values can be null if the CSV file only contains a header:
                                    continue;
                                }

                                if (values.Length < minLen)
                                {
                                    throw new Exception("Not enough fields in CSV file line.");
                                }

                                var newClassInit = Expression.MemberInit(
                                    Expression.New(classConstructor),
                                    classProperties.Select((p, i) =>
                                    {
                                        var value = values[i];
                                        var type = types[i];

                                        var valueConstant = type != typeof(string)
                                            ? Expression.Constant(Convert.ChangeType(value, type), type)
                                            : Expression.Constant(value);

                                        return Expression.Bind(p, valueConstant);
                                    }));

                                classObjects.Add(newClassInit);
                                values = parser.ReadFields();
                            } while (values != null);

                            fld.SetInitialValue(Expression.ListInit(
                                Expression.New(classListType.GetConstructor(Type.EmptyTypes)),
                                classObjects));
                        });
                    });
                });
            }
        }

        private static string GetClassName(string csvFilePath)
        {
            var csvFileName = Path.GetFileNameWithoutExtension(csvFilePath);

            if (csvFileName.EndsWith("ies", OrdinalIgnoreCase))
            {
                return csvFileName.Substring(0, csvFileName.Length - 3) + "y";
            }

            if (csvFileName.EndsWith("s", OrdinalIgnoreCase))
            {
                return csvFileName.Substring(0, csvFileName.Length - 1);
            }

            return csvFileName;
        }

        // Examines the header row and the first row in the csv file to gather all header types and names
        // Also it returns the first row of data, because it must be read to figure out the types,
        // As the CsvTextFieldParser cannot 'Peek' ahead of one line. If there is no first line,
        // it consider all properties as strings. The generator returns an empty list of properly
        // typed objects in such cas. If the file is completely empty, an error is generated.
        private static (Type[], string[], string[]) GetClassProperties(CsvTextFieldParser parser)
        {
            var headerFields = parser.ReadFields();

            if (headerFields == null)
            {
                throw new Exception("Empty csv file!");
            }

            var firstLineFields = parser.ReadFields();

            if (firstLineFields == null)
            {
                return (
                    Enumerable.Repeat(typeof(string), headerFields.Length).ToArray(),
                    headerFields,
                    firstLineFields);
            }

            return (
                firstLineFields.Select(GetCsvPropertyType).ToArray(),
                headerFields.Select(GetValidPropertyName).ToArray(),
                firstLineFields);
        }

        // Guesses type of property for the object from the value of a csv field
        private static Type GetCsvPropertyType(string exemplar) => exemplar switch
        {
            _ when bool.TryParse(exemplar, out _) => typeof(bool),
            _ when int.TryParse(exemplar, out _) => typeof(int),
            _ when double.TryParse(exemplar, out _) => typeof(double),
            _ => typeof(string)
        };

        private static string GetValidPropertyName(string s)
        {
            s = s.Trim();
            s = char.IsLetter(s[0]) ? char.ToUpper(s[0]) + s.Substring(1) : s;
            s = char.IsDigit(s.Trim()[0]) ? "_" + s : s;
            s = new string(s.Select(ch => char.IsDigit(ch) || char.IsLetter(ch) ? ch : '_').ToArray());
            return s;
        }
    }
}
