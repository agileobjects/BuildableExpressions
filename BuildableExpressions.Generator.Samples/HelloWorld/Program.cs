// This generator creates an OutputAssemblies class with a static All field, 
// initialised with the names of the assemblies in this project's build output folder.

// The code generation's build output is prefixed with 'XprBuilder.HelloWorld',
// which is set using this project's XprGeneratorLoggerPrefix property.
// This is a .NET 5.0 console app.

// This sample is based on the .NET 5 Source Generators sample:
// https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/HelloWorldGenerator.cs

namespace AgileObjects.BuildableExpressions.Generator.Samples.HelloWorld
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            Console.WriteLine();
            Console.WriteLine("This project has the following output assemblies:");
            Console.WriteLine();

            foreach (var assemblyName in OutputAssemblies.All)
            {
                Console.WriteLine("\t" + assemblyName);
            }

            Console.WriteLine();
            Console.WriteLine("This list was created at build time by the BuildableExpressions Generator!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
