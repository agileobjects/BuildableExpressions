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
