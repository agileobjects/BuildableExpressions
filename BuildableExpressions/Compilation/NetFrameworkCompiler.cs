#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CSharp;

    internal class NetFrameworkCompiler : ICompiler
    {
        public CompilationResult Compile(IEnumerable<string> expressionBuilderSources)
        {
            var codeProvider = new CSharpCodeProvider();
            var sources = expressionBuilderSources.ToArray();

            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false
            };

            var referenceAssemblyPaths = sources
                .SelectMany(ebs => ebs.GetReferenceAssemblyTypes())
                .Distinct()
                .Select(GetAssemblyFilePath)
                .ToArray();

            parameters.ReferencedAssemblies.AddRange(referenceAssemblyPaths);

            var compilationResult = codeProvider
                .CompileAssemblyFromSource(parameters, sources);

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

        private static string GetAssemblyFilePath(Type type) => type.Assembly.Location;
    }
}
#endif