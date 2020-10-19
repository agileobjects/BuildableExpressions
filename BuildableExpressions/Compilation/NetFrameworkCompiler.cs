#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CSharp;
    using static CompilationExtensions;

    internal class NetFrameworkCompiler : ICompiler
    {
        public CompilationResult Compile(
            IEnumerable<Assembly> referenceAssemblies,
            params string[] sourceCodes)
        {
            var codeProvider = new CSharpCodeProvider();

            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false
            };

            var referenceAssemblyPaths = CompilationAssemblies
                .Concat(referenceAssemblies)
                .Distinct()
                .Select(GetFilePath)
                .ToArray();

            parameters.ReferencedAssemblies.AddRange(referenceAssemblyPaths);

            var compilationResult = codeProvider
                .CompileAssemblyFromSource(parameters, sourceCodes);

            if (compilationResult.Errors.HasErrors)
            {
                return new CompilationResult
                {
                    Errors = compilationResult
                        .Errors
                        .Cast<CompilerError>()
                        .Select(ce => $"Error ({ce.ErrorNumber}): {ce.ErrorText}")
                        .ToList()
                };
            }

            return new CompilationResult
            {
                CompiledAssembly = compilationResult.CompiledAssembly
            };
        }

        private static string GetFilePath(Assembly assembly) => assembly.Location;
    }
}
#endif